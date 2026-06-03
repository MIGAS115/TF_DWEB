using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models;

/// <summary>
/// Perfil com privilégios de Administração, herda as credenciais do MyUser.
/// </summary>
public class Admin : MyUser
{
    /// <summary>
    /// Define o nível de permissões administrativas na plataforma de e-sports.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(50)]
    [Display(Name = "Nível de Permissão")]
    public string PermissionLevel { get; set; } = "FullAccess";
}