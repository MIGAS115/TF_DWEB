using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models;

/// <summary>
/// Entidade que representa um jogo ou partida entre duas equipas.
/// </summary>
public class Match
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Data e Hora do Jogo")]
    [Required]
    public DateTime MatchDate { get; set; }

    [Display(Name = "Resultado Equipa Casa")]
    public int? HomeScore { get; set; }

    [Display(Name = "Resultado Equipa Fora")]
    public int? AwayScore { get; set; }

    /// <summary>
    /// Indica se o registo foi criado ou editado manualmente por um administrador.
    /// </summary>
    public bool IsManualOverride { get; set; }

    /// <summary>
    /// Identificador do registo proveniente da API externa de dados.
    /// </summary>
    public string? ExternalSourceId { get; set; }

    /// <summary>
    /// Chave estrangeira para a equipa visitada (casa).
    /// </summary>
    [ForeignKey(nameof(HomeTeam))]
    [Display(Name = "Equipa Visitada")]
    public int HomeTeamFK { get; set; }

    [InverseProperty(nameof(Team.HomeMatches))]
    public Team HomeTeam { get; set; } = null!;

    /// <summary>
    /// Chave estrangeira para a equipa visitante (fora).
    /// </summary>
    [ForeignKey(nameof(AwayTeam))]
    [Display(Name = "Equipa Visitante")]
    public int AwayTeamFK { get; set; }

    [InverseProperty(nameof(Team.AwayMatches))]
    public Team AwayTeam { get; set; } = null!;
}