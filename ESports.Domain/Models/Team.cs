using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ESports.Domain.Models;

/// <summary>
/// Entidade que representa uma equipa de E-sports.
/// </summary>
public class Team
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O {0} é obrigatório")]
    [StringLength(50, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
    [Display(Name = "Nome da Equipa")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o registo foi inserido/editado manualmente (Redundância/Defesa).
    /// </summary>
    public bool IsManualOverride { get; set; }

    /// <summary>
    /// ID proveniente da API externa.
    /// </summary>
    public string? ExternalSourceId { get; set; }

    public ICollection<Match> HomeMatches { get; set; } = [];
    public ICollection<Match> AwayMatches { get; set; } = [];
    public ICollection<Favorite> FavoritesList { get; set; } = [];
}