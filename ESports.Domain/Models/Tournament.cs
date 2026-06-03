using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Entidade de domínio que representa um Torneio de desportos eletrónicos.
    /// Unifica a lógica híbrida e a segregação por jogos (CS2, LOL, DOTA2) através do atributo GameName.
    /// </summary>
    public class Tournament
    {
        /// <summary>
        /// Chave primária do Torneio.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome oficial do torneio (ex: Blast Premier Spring 2026).
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [StringLength(100, ErrorMessage = "O {0} não pode exceder os {1} caracteres.")]
        [Display(Name = "Nome do Torneio")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Identificador legível do jogo associado (ex: CS2, LOL, DOTA2).
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [StringLength(50, ErrorMessage = "O {0} não pode exceder os {1} caracteres.")]
        [Display(Name = "Nome do Jogo")]
        public string GameName { get; set; } = null!;

        /// <summary>
        /// Identificador único do registo de origem na API externa de e-sports.
        /// </summary>
        [StringLength(100, ErrorMessage = "O {0} não pode exceder os {1} caracteres.")]
        [Display(Name = "ID Fonte Externa")]
        public string? ExternalSourceId { get; set; }

        /// <summary>
        /// Indica se o registo foi inserido ou modificado manualmente via Administração.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Substituição Manual")]
        public bool IsManualOverride { get; set; }

        /// <summary>
        /// Valor real do prémio total do torneio, guardado na base de dados.
        /// </summary>
        [Precision(18, 2)]
        [Display(Name = "Prémio Total")]
        public decimal PrizePool { get; set; }

        /// <summary>
        /// Propriedade auxiliar para capturar o input do formulário em formato de texto (suporta ponto e vírgula).
        /// </summary>
        [NotMapped]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [StringLength(10, ErrorMessage = "O {0} não pode exceder os {1} caracteres.")]
        [RegularExpression(@"[0-9]{1,7}([,.][0-9]{1,2})?", ErrorMessage = "O {0} deve ser um número válido com até 2 casas decimais.")]
        [Display(Name = "Prémio Total")]
        public string PrizePoolAux { get; set; } = "";

        /// <summary>
        /// Lista de jogos e partidas integrados e realizados dentro deste torneio (Relação 1:N).
        /// </summary>
        [ValidateNever]
        [Display(Name = "Partidas Associadas")]
        public ICollection<Match> MatchesList { get; set; } = [];

        /// <summary>
        /// Coleção de registos de junção que mapeia as equipas inscritas neste torneio (Relação M:N).
        /// </summary>
        [ValidateNever]
        public ICollection<TournamentTeam> TournamentTeams { get; set; } = [];
    }
}