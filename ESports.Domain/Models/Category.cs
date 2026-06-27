using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models;

/// <summary>
/// Representa uma categoria de jogo (ex: CS2, LOL, DOTA2) na plataforma de e-sports.
/// </summary>
public class Category
{
    /// <summary>
    /// Identificador único da categoria.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nome da categoria do jogo (ex: Counter-Strike 2).
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(50, ErrorMessage = "O {0} deve ter no máximo {1} caracteres.")]
    [Display(Name = "Nome da Categoria")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Código abreviado, sigla ou acrónimo da categoria do jogo (ex: CS2, LOL, DOTA2).
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(10, ErrorMessage = "O {0} deve ter no máximo {1} caracteres.")]
    [Display(Name = "Código da Categoria")]
    public string Code { get; set; } = string.Empty;

    /* ****************************************
     * Construção dos Relacionamentos
     * *************************************** */

    /// <summary>
    /// Lista de equipas que pertencem a esta categoria.
    /// Inicializada vazia de acordo com as regras de estilo para coleções.
    /// </summary>
    public ICollection<Team> TeamsList { get; set; } = [];
}