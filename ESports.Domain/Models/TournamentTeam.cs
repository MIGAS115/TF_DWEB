using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Classe de associação (tabela pivot) que modela o relacionamento Muitos-para-Muitos (N:M) 
    /// entre as entidades Torneio e Equipa (Tabela TorneioEquipa).
    /// </summary>
    [PrimaryKey(nameof(TorneioFK), nameof(EquipaFK))]
    public class TournamentTeam
    {
        /// <summary>
        /// Chave estrangeira e parte da chave primária composta que aponta para o Torneio.
        /// </summary>
        [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
        [ForeignKey(nameof(Tournament))]
        [Display(Name = "Identificador do Torneio")]
        public int TorneioFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para o objeto Torneio associado.
        /// </summary>
        [ValidateNever]
        public Tournament Tournament { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira e parte da chave primária composta que aponta para a Equipa.
        /// </summary>
        [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
        [ForeignKey(nameof(Team))]
        [Display(Name = "Identificador da Equipa")]
        public int EquipaFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para o objeto Equipa associado.
        /// </summary>
        [ValidateNever]
        public Team Team { get; set; } = null!;
    }
}