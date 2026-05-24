using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Entidade intermédia que representa o relacionamento Muitos-para-Muitos (N:M) 
    /// entre um Utilizador do tipo Normal e as suas Equipas favoritas.
    /// </summary>
    [PrimaryKey(nameof(UserFK), nameof(TeamFK))]
    public class Favorite
    {
        /// <summary>
        /// Chave estrangeira associada ao Utilizador do tipo Normal.
        /// </summary>
        [ForeignKey(nameof(Normal))]
        public string UserFK { get; set; } = string.Empty;

        /// <summary>
        /// Propriedade de navegação para a entidade do Utilizador Normal.
        /// </summary>
        public Normal Normal { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira associada à Equipa preferida.
        /// </summary>
        [ForeignKey(nameof(Team))]
        public int TeamFK { get; set; }

        /// <summary>
        /// Propriedade de navegação para a entidade da Equipa.
        /// </summary>
        public Team Team { get; set; } = null!;
    }
}