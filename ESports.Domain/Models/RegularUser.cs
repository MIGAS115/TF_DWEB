using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Perfil de utilizador normal da plataforma (Armazena dados de negócio).
    /// </summary>
    public class RegularUser
    {
        /// <summary>
        /// Identificador único do perfil normal.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Data em que o utilizador se registou.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Data de Registo")]
        public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Chave Estrangeira para o utilizador de autenticação (Identity).
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Utilizador Identity")]
        public string MyUserFK { get; set; } = null!;

        /// <summary>
        /// Propriedade de navegação para o utilizador do Identity.
        /// </summary>
        [ForeignKey(nameof(MyUserFK))]
        public MyUser MyUser { get; set; } = null!;

        /* ****************************************
         * Construção dos Relacionamentos
         * *************************************** */

        /// <summary>
        /// Lista de equipas favoritas associadas a este perfil.
        /// </summary>
        public ICollection<Favorite> FavoritesList { get; set; } = [];
    }
}