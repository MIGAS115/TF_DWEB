using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Entidade de domínio que representa uma partida ou jogo entre duas equipas num torneio.
    /// Implementa o suporte a dados híbridos e chaves estrangeiras duplas auto-referenciais.
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Chave primária da partida.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Data de realização da partida.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [Display(Name = "Data da Partida")]
        public DateTime MatchDate { get; set; }

        /// <summary>
        /// Pontuação obtida pela equipa visitada (casa). Nullable se o jogo não ocorreu.
        /// </summary>
        [Display(Name = "Pontuação Casa")]
        [Range(0, 500, ErrorMessage = "A {0} deve ser um valor positivo.")]
        public int? ScoreTeam1 { get; set; }

        /// <summary>
        /// Pontuação obtida pela equipa visitante (fora). Nullable se o jogo não ocorreu.
        /// </summary>
        [Display(Name = "Pontuação Fora")]
        [Range(0, 500, ErrorMessage = "A {0} deve ser um valor positivo.")]
        public int? ScoreTeam2 { get; set; }

        /// <summary>
        /// Estado atual do jogo (ex: Agendado, Ao Vivo, Terminado).
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "O {0} não pode exceder os {1} caracteres.")]
        [Display(Name = "Estado do Jogo")]
        public string Status { get; set; } = null!;

        /// <summary>
        /// Indica se o registo foi manipulado manualmente via administração.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Substituição Manual")]
        public bool IsManualOverride { get; set; }

        /// <summary>
        /// Identificador único do registo na API externa de e-sports.
        /// </summary>
        [StringLength(100, ErrorMessage = "O {0} não pode exceder os {1} caracteres.")]
        [Display(Name = "ID Fonte Externa")]
        public string? ExternalSourceId { get; set; }

        /// <summary>
        /// Chave estrangeira para a equipa visitada (casa).
        /// </summary>
        [Required(ErrorMessage = "A associação à {0} é obrigatória.")]
        [ForeignKey(nameof(HomeTeam))]
        [Display(Name = "Equipa Casa")]
        public int HomeTeamFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para a equipa visitada.
        /// </summary>
        [ValidateNever]
        [InverseProperty(nameof(Team.HomeMatches))]
        public Team HomeTeam { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira para a equipa visitante (fora).
        /// </summary>
        [Required(ErrorMessage = "A associação à {0} é obrigatória.")]
        [ForeignKey(nameof(AwayTeam))]
        [Display(Name = "Equipa Fora")]
        public int AwayTeamFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para a equipa visitante.
        /// </summary>
        [ValidateNever]
        [InverseProperty(nameof(Team.AwayMatches))]
        public Team AwayTeam { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira para o Torneio correspondente.
        /// </summary>
        [Required(ErrorMessage = "A associação a um {0} é obrigatória.")]
        [ForeignKey(nameof(Tournament))]
        [Display(Name = "Torneio")]
        public int TournamentFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para o Torneio onde a partida está inserida.
        /// </summary>
        [ValidateNever]
        public Tournament Tournament { get; set; } = null!;
    }
}