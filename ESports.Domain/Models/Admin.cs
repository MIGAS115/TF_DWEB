using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Perfil com privilégios de Administração.
    /// </summary>
    public class Admin
    {
        /// <summary>
        /// Identificador único do Administrador.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nível de permissão atribuído ao administrador (ex: FullAccess, Moderator).
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50)]
        [Display(Name = "Nível de Permissão")]
        public string PermissionLevel { get; set; } = "FullAccess";

        /// <summary>
        /// Chave estrangeira de associação ao utilizador de Identity (MyUser).
        /// </summary>
        [Required]
        public string MyUserFK { get; set; } = null!;

        /// <summary>
        /// Propriedade de navegação para o utilizador de Identity associado.
        /// </summary>
        [ForeignKey(nameof(MyUserFK))]
        public MyUser MyUser { get; set; } = null!;
    }
}