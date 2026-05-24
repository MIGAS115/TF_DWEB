namespace WebApi.Models.ViewModels;

/// <summary>
/// Objeto de transferência de dados (DTO) utilizado para expor as informações detalhadas e estatísticas de um Torneio.
/// </summary>
public class TournamentDTO
{
    /// <summary>
    /// Identificador único do torneio na base de dados.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome descritivo do torneio ou competição de e-sports.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Total de jogos (matches) associados ou agendados para este torneio.
    /// </summary>
    public int TotalMatches { get; set; }

    /// <summary>
    /// Total de equipas atualmente inscritas ou participantes neste torneio.
    /// </summary>
    public int TotalTeams { get; set; }
}