using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ESports.Domain.Models;

/// <summary>
/// Tabela intermédia N:M entre Utilizadores e Equipas.
/// </summary>
[PrimaryKey(nameof(MyUserFK), nameof(TeamFK))]
public class UserFavoriteTeam
{
    [Display(Name = "Data de Adição aos Favoritos")]
    public DateTime AddedAt { get; set; } = DateTime.Now;

    [ForeignKey(nameof(User))]
    public string MyUserFK { get; set; } = null!;
    public MyUser User { get; set; } = null!;

    [ForeignKey(nameof(Team))]
    public int TeamFK { get; set; }
    public Team Team { get; set; } = null!;
}