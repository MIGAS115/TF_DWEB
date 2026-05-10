using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Representa um Torneio
    /// </summary>
    public class Tournament
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório!")]
        [StringLength(100)]
        [Display(Name = "Torneio")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório!")]
        [StringLength(50)]
        [Display(Name = "Jogo")]
        public string GameName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ExternalSourceId { get; set; }

        public bool IsManualOverride { get; set; }

        /* Relacionamentos */
        public ICollection<Match> MatchesList { get; set; } = [];
        // NOTA: Como ainda não têm o modelo TournamentTeam, podem deixar a linha abaixo comentada por agora, ou criar a classe no passo 2
        // public ICollection<TournamentTeam> TeamsList { get; set; } = [];
    }
}