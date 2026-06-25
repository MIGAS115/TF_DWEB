using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Classe de associação (tabela pivot) que modela o relacionamento Muitos-para-Muitos (N:M) 
    /// entre Utilizadores do tipo Normal e as suas Equipas favoritas.
    /// </summary>
    [Table("Favoritos")]
    [PrimaryKey(nameof(NormalFK), nameof(TeamFK))]
    public class Favorite
    {
        /// <summary>
        /// Chave estrangeira e parte da chave primária composta que aponta para o Utilizador Normal.
        /// Alterado para string para refletir a herança do Identity Framework.
        /// </summary>
        [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
        [ForeignKey(nameof(Normal))]
        [Display(Name = "Identificador do Utilizador")]
        public int NormalFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para a entidade do Utilizador Normal associado.
        /// </summary>
        [ValidateNever]
        public RegularUser Normal { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira e parte da chave primária composta que aponta para a Equipa.
        /// </summary>
        [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
        [ForeignKey(nameof(Team))]
        [Display(Name = "Identificador da Equipa")]
        public int TeamFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para a entidade da Equipa associada.
        /// </summary>
        [ValidateNever]
        public Team Team { get; set; } = null!;
    }
}