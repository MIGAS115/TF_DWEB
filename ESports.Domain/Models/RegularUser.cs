using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models
{

    /// <summary>
    /// Utilizador Normal da plataforma (Herda de MyUser)
    /// </summary>
    public class Normal : MyUser
    {

        /// <summary>
        /// Data em que o utilizador se registou
        /// </summary>
        [DataType(DataType.Date)]
        public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        /* ****************************************
         * Construção dos Relacionamentos
         * *************************************** */
        public ICollection<Favorite> FavoritesList { get; set; } = [];
    }
}