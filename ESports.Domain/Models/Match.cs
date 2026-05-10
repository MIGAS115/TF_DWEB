using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models;

/// <summary>
/// Representa um jogo entre duas equipas.
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

    public bool IsManualOverride { get; set; }
    public string? ExternalSourceId { get; set; }

    /* RELACIONAMENTO 1: EQUIPA DA CASA */
    [ForeignKey(nameof(HomeTeam))]
    [Display(Name = "Equipa Visitada")]
    public int HomeTeamFK { get; set; }

    [InverseProperty(nameof(Team.HomeMatches))] // <-- ESTA É A LINHA MÁGICA
    public Team HomeTeam { get; set; } = null!;

    /* RELACIONAMENTO 2: EQUIPA DE FORA */
    [ForeignKey(nameof(AwayTeam))]
    [Display(Name = "Equipa Visitante")]
    public int AwayTeamFK { get; set; }

    [InverseProperty(nameof(Team.AwayMatches))] // <-- E ESTA TAMBÉM
    public Team AwayTeam { get; set; } = null!;
}