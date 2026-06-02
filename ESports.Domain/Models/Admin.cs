using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Perfil com privilégios de Administração.
    /// </summary>
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50)]
        [Display(Name = "Nível de Permissão")]
        public string PermissionLevel { get; set; } = "FullAccess";

        [Required]
        public string MyUserFK { get; set; } = null!;

        [ForeignKey(nameof(MyUserFK))]
        public MyUser MyUser { get; set; } = null!;
    }
}