using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models;

/// <summary>
/// Entidade que representa um jogo ou partida entre duas equipas na plataforma de e-sports.
/// </summary>
public class Match
{
    /// <summary>
    /// Identificador único do jogo na base de dados.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Data e hora agendada para a realização da partida.
    /// </summary>
    [Display(Name = "Data e Hora do Jogo")]
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
    public DateTime MatchDate { get; set; }

    /// <summary>
    /// Pontuação ou número de mapas/rondas vencidos pela equipa visitada (Casa).
    /// </summary>
    [Display(Name = "Resultado Equipa Casa")]
    public int? HomeScore { get; set; }

    /// <summary>
    /// Pontuação ou número de mapas/rondas vencidos pela equipa visitante (Fora).
    /// </summary>
    [Display(Name = "Resultado Equipa Fora")]
    public int? AwayScore { get; set; }

    /// <summary>
    /// Sinalizador (flag) que indica se o registo foi criado ou editado manualmente por um administrador, sobrepondo os dados automáticos.
    /// </summary>
    public bool IsManualOverride { get; set; }

    /// <summary>
    /// Identificador alfanumérico do registo proveniente da API externa de dados de e-sports.
    /// </summary>
    public string? ExternalSourceId { get; set; }

    /// <summary>
    /// Chave estrangeira para a equipa visitada (casa).
    /// </summary>
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
    [ForeignKey(nameof(HomeTeam))]
    [Display(Name = "Equipa Visitada")]
    public int HomeTeamFK { get; set; }

    /// <summary>
    /// Propriedade de navegação para os detalhes da equipa visitada (casa).
    /// </summary>
    [InverseProperty(nameof(Team.HomeMatches))]
    public Team HomeTeam { get; set; } = null!;

    /// <summary>
    /// Chave estrangeira para a equipa visitante (fora).
    /// </summary>
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
    [ForeignKey(nameof(AwayTeam))]
    [Display(Name = "Equipa Visitante")]
    public int AwayTeamFK { get; set; }

    /// <summary>
    /// Propriedade de navegação para os detalhes da equipa visitante (fora).
    /// </summary>
    [InverseProperty(nameof(Team.AwayMatches))]
    public Team AwayTeam { get; set; } = null!;

    /// <summary>
    /// Chave estrangeira para o Torneio onde o jogo decorre.
    /// </summary>
    [Required(ErrorMessage = "O Torneio é de preenchimento obrigatório.")]
    [ForeignKey(nameof(Tournament))]
    [Display(Name = "Torneio")]
    public int TournamentFK { get; set; }

    /// <summary>
    /// Propriedade de navegação para os detalhes do torneio associado.
    /// </summary>
    public Tournament Tournament { get; set; } = null!;
}