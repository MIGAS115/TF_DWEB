using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models;

/// <summary>
/// Perfil de utilizador regular da plataforma.
/// Armazena dados específicos do ecossistema de e-sports e herda de MyUser.
/// </summary>
public class RegularUser : MyUser
{
    /// <summary>
    /// Data em que o utilizador criou a sua conta na plataforma.
    /// Utiliza DateOnly para representar datas puras sem componente horária.
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de Registo")]
    public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    /* ************************************************************
     * Relacionamentos de Negócio
     * ************************************************************ */

    /// <summary>
    /// Lista de associações de equipas favoritas marcadas por este utilizador.
    /// Inicializada como uma coleção vazia para evitar exceções de referência nula.
    /// </summary>
    public ICollection<Favorite> FavoritesList { get; set; } = [];
}