using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models
{

    /// <summary>
    /// Relacionamento M:N entre Utilizador Normal e Equipa
    /// </summary>
    [PrimaryKey(nameof(NormalUserFK), nameof(TeamFK))]
    public class Favorite
    {

        [ForeignKey(nameof(Normal))]
        public string NormalUserFK { get; set; } = string.Empty;
        public Normal Normal { get; set; } = null!;

        [ForeignKey(nameof(Team))]
        public int TeamFK { get; set; }
        public Team Team { get; set; } = null!;
    }
}