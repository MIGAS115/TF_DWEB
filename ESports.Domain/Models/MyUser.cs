using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models;

/// <summary>
/// Extensão abstrata do IdentityUser. 
/// Define a base para a herança em TPH (Table-Per-Hierarchy) para diferentes perfis.
/// </summary>
public abstract class MyUser : IdentityUser
{
    /// <summary>
    /// Nome completo do utilizador na plataforma.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 3)]
    [Display(Name = "Nome Completo")]
    public string FullName { get; set; } = null!;
}