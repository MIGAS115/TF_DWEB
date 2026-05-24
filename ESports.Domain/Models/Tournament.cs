using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Entidade que representa um Torneio de desportos eletrónicos.
    /// </summary>
    public class Tournament
    {
        /// <summary>
        /// Chave primária do Torneio.
        /// </summary>
        [Key]
        public int Id { get; set; }

        // CORREÇÃO: Adequação da string à máscara formal do Politécnico de Tomar
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(100, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
        [Display(Name = "Nome do Torneio")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
        [Display(Name = "Nome do Jogo")]
        public string GameName { get; set; } = null!;

        /// <summary>
        /// ID de identificação da API externa.
        /// </summary>
        [StringLength(100)]
        public string? ExternalSourceId { get; set; }

        /// <summary>
        /// Controla se os dados sofreram modificação manual.
        /// </summary>
        public bool IsManualOverride { get; set; }

        /// <summary>
        /// Lista de jogos integrados neste torneio.
        /// </summary>
        public ICollection<Match> MatchesList { get; set; } = [];
    }
}