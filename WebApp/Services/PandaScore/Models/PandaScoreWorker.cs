using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApp.Hubs;

namespace WebApp.Services.PandaScore;

/// <summary>
/// Serviço de background responsável por consultar a API da PandaScore periodicamente.
/// Processa atualizações de jogos, sincroniza entidades em modo híbrido e dispara eventos via SignalR.
/// </summary>
public class PandaScoreWorker : BackgroundService
{
    private readonly ILogger<PandaScoreWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<MatchHub> _hubContext;
    private readonly IConfiguration _configuration;
    private readonly string _targetFolder;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="PandaScoreWorker"/>.
    /// </summary>
    /// <param name="logger">Instância do sistema de registo de logs.</param>
    /// <param name="httpClientFactory">Fábrica para criação de instâncias de HttpClient.</param>
    /// <param name="serviceProvider">Provedor de serviços para resolução de dependências em escopo.</param>
    /// <param name="hubContext">Contexto do SignalR para comunicação em tempo real.</param>
    /// <param name="configuration">Instância das configurações da aplicação.</param>
    public PandaScoreWorker(
        ILogger<PandaScoreWorker> logger,
        IHttpClientFactory httpClientFactory,
        IServiceProvider serviceProvider,
        IHubContext<MatchHub> hubContext,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _configuration = configuration;
        _targetFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
    }

    /// <summary>
    /// Ciclo de execução contínua do serviço em segundo plano.
    /// </summary>
    /// <param name="stoppingToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>Uma Task representando a execução do serviço.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Serviço PandaScore iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPandaScoreMatchesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro interno ao tentar processar os dados da PandaScore.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    /// <summary>
    /// Faz o pedido HTTP à API externa, converte o JSON e atualiza as entidades na base de dados.
    /// </summary>
    /// <param name="stoppingToken">Token de cancelamento da operação assíncrona.</param>
    private async Task ProcessPandaScoreMatchesAsync(CancellationToken stoppingToken)
    {
        var apiKey = _configuration["PandaScore:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("ApiKey da PandaScore não está configurada no appsettings.json.");
            return;
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("https://api.pandascore.co/matches?sort=&page=1&per_page=50", stoppingToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("A API da PandaScore retornou o status: {StatusCode}", response.StatusCode);
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync(stoppingToken);
        var matchesFromApi = JsonSerializer.Deserialize<List<PandaScoreMatchDTO>>(jsonString);

        if (matchesFromApi == null || matchesFromApi.Count == 0) return;

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        foreach (var apiMatch in matchesFromApi)
        {
            if (apiMatch.Opponents == null || apiMatch.Opponents.Count < 2 || apiMatch.Tournament == null || apiMatch.Videogame == null)
            {
                continue;
            }

            var categoryName = apiMatch.Videogame.Name.ToUpperInvariant();
            string internalCategoryName = "CS2";

            if (categoryName.Contains("LEAGUE OF LEGENDS") || categoryName.Contains("LOL"))
            {
                internalCategoryName = "LOL";
            }
            else if (categoryName.Contains("DOTA"))
            {
                internalCategoryName = "DOTA2";
            }

            var category = await context.Categories.FirstOrDefaultAsync(c => c.Name == internalCategoryName, stoppingToken);
            if (category == null) continue;

            var tournament = await context.Tournaments.FirstOrDefaultAsync(t => t.ExternalSourceId == apiMatch.Tournament.Id.ToString(), stoppingToken);
            if (tournament == null)
            {
                tournament = new Tournament
                {
                    Name = apiMatch.Tournament.Name,
                    GameName = internalCategoryName,
                    ExternalSourceId = apiMatch.Tournament.Id.ToString(),
                    IsManualOverride = false
                };
                context.Tournaments.Add(tournament);
                await context.SaveChangesAsync(stoppingToken);
            }

            var apiHomeTeam = apiMatch.Opponents[0].Team;
            var apiAwayTeam = apiMatch.Opponents[1].Team;

            if (apiHomeTeam == null || apiAwayTeam == null) continue;

            var homeTeam = await context.Teams.FirstOrDefaultAsync(t => t.ExternalSourceId == apiHomeTeam.Id.ToString(), stoppingToken);
            if (homeTeam == null)
            {
                var localLogo = await DownloadLogoAsync(client, apiHomeTeam.ImageUrl);
                homeTeam = new Team
                {
                    Name = apiHomeTeam.Name,
                    CategoryFK = category.Id,
                    ExternalSourceId = apiHomeTeam.Id.ToString(),
                    IsManualOverride = false,
                    LogoPath = localLogo
                };
                context.Teams.Add(homeTeam);
                await context.SaveChangesAsync(stoppingToken);
            }

            var awayTeam = await context.Teams.FirstOrDefaultAsync(t => t.ExternalSourceId == apiAwayTeam.Id.ToString(), stoppingToken);
            if (awayTeam == null)
            {
                var localLogo = await DownloadLogoAsync(client, apiAwayTeam.ImageUrl);
                awayTeam = new Team
                {
                    Name = apiAwayTeam.Name,
                    CategoryFK = category.Id,
                    ExternalSourceId = apiAwayTeam.Id.ToString(),
                    IsManualOverride = false,
                    LogoPath = localLogo
                };
                context.Teams.Add(awayTeam);
                await context.SaveChangesAsync(stoppingToken);
            }

            var dbMatch = await context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.ExternalSourceId == apiMatch.Id.ToString(), stoppingToken);

            if (dbMatch == null)
            {
                dbMatch = new Match
                {
                    MatchDate = apiMatch.ScheduledAt.ToLocalTime(),
                    Status = apiMatch.Status,
                    ExternalSourceId = apiMatch.Id.ToString(),
                    IsManualOverride = false,
                    HomeTeamFK = homeTeam.Id,
                    AwayTeamFK = awayTeam.Id,
                    TournamentFK = tournament.Id
                };

                if (apiMatch.Results != null && apiMatch.Results.Count > 0)
                {
                    var homeResult = apiMatch.Results.FirstOrDefault(r => r.TeamId == apiHomeTeam.Id);
                    var awayResult = apiMatch.Results.FirstOrDefault(r => r.TeamId == apiAwayTeam.Id);
                    if (homeResult != null) dbMatch.HomeScore = homeResult.Score;
                    if (awayResult != null) dbMatch.AwayScore = awayResult.Score;
                }

                context.Matches.Add(dbMatch);
                await context.SaveChangesAsync(stoppingToken);

                await _hubContext.Clients.All.SendAsync(
                    "ReceiveNewMatch",
                    dbMatch.Id,
                    internalCategoryName,
                    $"{homeTeam.Name} vs {awayTeam.Name}",
                    cancellationToken: stoppingToken);
            }
            else
            {
                if (!dbMatch.IsManualOverride)
                {
                    bool wasUpdated = false;

                    if (dbMatch.Status != apiMatch.Status)
                    {
                        dbMatch.Status = apiMatch.Status;
                        wasUpdated = true;
                    }

                    if (dbMatch.MatchDate != apiMatch.ScheduledAt.ToLocalTime())
                    {
                        dbMatch.MatchDate = apiMatch.ScheduledAt.ToLocalTime();
                        wasUpdated = true;
                    }

                    if (apiMatch.Results != null && apiMatch.Results.Count > 0)
                    {
                        var homeResult = apiMatch.Results.FirstOrDefault(r => r.TeamId == apiHomeTeam.Id);
                        var awayResult = apiMatch.Results.FirstOrDefault(r => r.TeamId == apiAwayTeam.Id);

                        if (homeResult != null && dbMatch.HomeScore != homeResult.Score)
                        {
                            dbMatch.HomeScore = homeResult.Score;
                            wasUpdated = true;
                        }

                        if (awayResult != null && dbMatch.AwayScore != awayResult.Score)
                        {
                            dbMatch.AwayScore = awayResult.Score;
                            wasUpdated = true;
                        }
                    }

                    if (wasUpdated)
                    {
                        await context.SaveChangesAsync(stoppingToken);

                        await _hubContext.Clients.All.SendAsync(
                            "ReceiveMatchUpdate",
                            dbMatch.Id,
                            dbMatch.Status,
                            dbMatch.HomeScore ?? 0,
                            dbMatch.AwayScore ?? 0,
                            cancellationToken: stoppingToken);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Descarrega a imagem do logótipo de uma equipa a partir de um URL remoto e armazena-a no servidor.
    /// </summary>
    /// <param name="client">O cliente HTTP utilizado para efetuar o download.</param>
    /// <param name="url">O URL absoluto da imagem remota.</param>
    /// <returns>O nome único do ficheiro gravado ou uma string vazia em caso de falha.</returns>
    private async Task<string> DownloadLogoAsync(HttpClient client, string? url)
    {
        if (string.IsNullOrEmpty(url)) return string.Empty;

        try
        {
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return string.Empty;

            var uri = new Uri(url);
            var fileExtension = Path.GetExtension(uri.AbsolutePath).ToLowerInvariant();

            if (string.IsNullOrEmpty(fileExtension))
            {
                fileExtension = ".png";
            }

            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var fullPath = Path.Combine(_targetFolder, uniqueFileName);

            if (!Directory.Exists(_targetFolder))
            {
                Directory.CreateDirectory(_targetFolder);
            }

            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(fullPath, imageBytes);

            return uniqueFileName;
        }
        catch
        {
            return string.Empty;
        }
    }
}