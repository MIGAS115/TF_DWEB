using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models;

/// <summary>
/// Perfil de utilizador normal da plataforma, herda as credenciais do MyUser.
/// Armazena dados específicos de negócio (e-sports).
/// </summary>
public class RegularUser : MyUser
{
    /// <summary>
    /// Data em que o utilizador se registou. 
    /// Utiliza DateOnly para representar datas puras (sem componente horária).
    /// </summary>
    [DataType(DataType.Date)]
    [Display(Name = "Data de Registo")]
    public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    /* ****************************************
     * Construção dos Relacionamentos
     * *************************************** */

    /// <summary>
    /// Lista de equipas favoritas associadas a este perfil.
    /// Inicializada com [] de acordo com o guia de estilo.
    /// </summary>
    public ICollection<Favorite> FavoritesList { get; set; } = [];
}