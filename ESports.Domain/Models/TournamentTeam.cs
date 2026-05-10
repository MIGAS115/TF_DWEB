using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Relacionamento M:N entre Torneio e Equipa
    /// </summary>
    [PrimaryKey(nameof(TournamentFK), nameof(TeamFK))]
    public class TournamentTeam
    {
        [ForeignKey(nameof(Tournament))]
        public int TournamentFK { get; set; }
        public Tournament Tournament { get; set; } = null!;

        [ForeignKey(nameof(Team))]
        public int TeamFK { get; set; }
        public Team Team { get; set; } = null!;
    }
}