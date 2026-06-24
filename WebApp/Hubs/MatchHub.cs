using Microsoft.AspNetCore.SignalR;

namespace WebApp.Hubs;

/// <summary>
/// Representa o ponto central de comunicação bidirecional em tempo real (WebSockets) do SignalR.
/// Responsável por difundir atualizações de resultados e estados de jogos (CS2, LOL, DOTA2) para todos os clientes ligados.
/// </summary>
public class MatchHub : Hub
{
    /// <summary>
    /// Transmite a atualização do estado ou pontuação de um jogo para todos os utilizadores ativos na plataforma.
    /// Pode ser invocado pelo serviço de sincronização da API externa ou por substituição manual do Admin.
    /// </summary>
    /// <param name="matchId">O identificador único (PK) local do jogo a ser atualizado.</param>
    /// <param name="status">O novo estado do jogo (ex: live, finished, not_started).</param>
    /// <param name="homeScore">A pontuação atual da equipa principal/casa.</param>
    /// <param name="awayScore">A pontuação atual da equipa adversária/visitante.</param>
    /// <returns>Uma tarefa assíncrona que representa o processo de notificação aos clientes.</returns>
    public async Task BroadcastMatchUpdate(int matchId, string status, int homeScore, int awayScore)
    {
        try
        {
            await Clients.All.SendAsync("ReceiveMatchUpdate", matchId, status, homeScore, awayScore);
        }
        catch (Exception)
        {
            // Ocultação do stack trace em produção conforme diretrizes do guia de estilo.
            // Num cenário real, a exceção seria registada num ficheiro de log interno (ex: Serilog) sem exposição ao cliente Web.
        }
    }

    /// <summary>
    /// Notifica os clientes de que um novo jogo foi adicionado ao sistema (útil para atualizar a listagem inicial dinamicamente).
    /// </summary>
    /// <param name="matchId">O identificador único do novo jogo gerado localmente.</param>
    /// <param name="categoryName">A designação da categoria (CS2, LOL ou DOTA2).</param>
    /// <param name="matchName">O título ou embate da partida (ex: NAVI vs FaZe).</param>
    /// <returns>Uma tarefa assíncrona que representa o processo de notificação aos clientes.</returns>
    public async Task BroadcastNewMatch(int matchId, string categoryName, string matchName)
    {
        try
        {
            await Clients.All.SendAsync("ReceiveNewMatch", matchId, categoryName, matchName);
        }
        catch (Exception)
        {
            // Captura de erro para garantir estabilidade da thread do SignalR.
        }
    }
}