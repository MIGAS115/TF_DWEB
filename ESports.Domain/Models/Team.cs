using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ESports.Domain.Models;

/// <summary>
/// Entidade que representa uma equipa de E-sports.
/// </summary>
public class Team
{
    /// <summary>
    /// Chave primária da equipa.
    /// </summary>
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(50, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
    [Display(Name = "Nome da Equipa")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Caminho do ficheiro do logótipo da equipa.
    /// </summary> 
    [Display(Name = "Logótipo")]
    [StringLength(255)]
    public string? LogoPath { get; set; }

    /// <summary>
    /// Identificador da origem externa dos dados se aplicável.
    /// </summary>
    public string? ExternalSourceId { get; set; }

    /// <summary>
    /// Indica se o registo foi inserido ou modificado manualmente pelo administrador.
    /// </summary>
    public bool IsManualOverride { get; set; }

    /// <summary>
    /// Coleção de jogos efetuados em casa.
    /// </summary>
    public ICollection<Match> HomeMatches { get; set; } = [];

    /// <summary>
    /// Coleção de jogos efetuados fora.
    /// </summary>
    public ICollection<Match> AwayMatches { get; set; } = [];

    /// <summary>
    /// Coleção de utilizadores que marcaram esta equipa como favorita.
    /// </summary>
    public ICollection<Favorite> FavoritedBy { get; set; } = [];
}