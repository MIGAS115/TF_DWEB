using ESports.Domain.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApp.Hubs;
namespace WebApp.Services.PandaScore;

/// <summary>
/// Serviço de background responsável por consultar a API da PandaScore periodicamente.
/// Processa atualizações de jogos e dispara eventos via SignalR para o frontend.
/// </summary>
public class PandaScoreWorker : BackgroundService
{
    private readonly ILogger<PandaScoreWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<MatchHub> _hubContext;
    private readonly IConfiguration _configuration;

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
    }

    /// <summary>
    /// Ciclo de execução contínua do serviço em segundo plano.
    /// </summary>
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
                // Regra do Guia: Tratamento de erros limpo. Nunca disparar throws cruas em produção.
                _logger.LogError(ex, "Ocorreu um erro interno ao tentar processar os dados da PandaScore.");
            }

            // Polling: Aguarda 1 minuto antes de fazer o próximo pedido
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    /// <summary>
    /// Faz o pedido HTTP à API externa, converte o JSON e atualiza a base de dados.
    /// </summary>
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

        // Faz o pedido aos jogos (Exemplo: limite de 50 jogos recentes/ao vivo)
        var response = await client.GetAsync("https://api.pandascore.co/matches?sort=&page=1&per_page=50", stoppingToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("A API da PandaScore retornou o status: {StatusCode}", response.StatusCode);
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync(stoppingToken);
        var matchesFromApi = JsonSerializer.Deserialize<List<PandaScoreMatchDTO>>(jsonString);

        if (matchesFromApi == null || matchesFromApi.Count == 0) return;

        // É necessário criar um scope porque o ApplicationDbContext é Scoped e o BackgroundService é Singleton
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        foreach (var apiMatch in matchesFromApi)
        {
            // Procura o jogo na nossa Base de Dados pelo ID da fonte externa
            var dbMatch = await context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.ExternalSourceId == apiMatch.Id.ToString(), stoppingToken);

            if (dbMatch != null)
            {
                // MODO HÍBRIDO: Só atualiza se a substituição manual estiver desligada
                if (!dbMatch.IsManualOverride)
                {
                    bool wasUpdated = false;

                    if (dbMatch.Status != apiMatch.Status)
                    {
                        dbMatch.Status = apiMatch.Status;
                        wasUpdated = true;
                    }

                    // Tenta mapear os resultados para a Equipa da Casa e Equipa Visitante
                    if (apiMatch.Results != null && apiMatch.Results.Count > 0)
                    {
                        var homeResult = apiMatch.Results.FirstOrDefault(r => dbMatch.HomeTeam.ExternalSourceId == r.TeamId.ToString());
                        var awayResult = apiMatch.Results.FirstOrDefault(r => dbMatch.AwayTeam.ExternalSourceId == r.TeamId.ToString());

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

                        // Dispara o SignalR para atualizar o ecrã instantaneamente
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
}