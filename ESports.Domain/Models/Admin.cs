using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Utilizador com privilégios de Administração (Herda de MyUser)
    /// </summary>
    public class Admin : MyUser
    {
        [Required]
        [StringLength(50)]
        public string PermissionLevel { get; set; } = "FullAccess";
    }
}