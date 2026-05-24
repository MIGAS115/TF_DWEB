using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.ViewModels;

/// <summary>
/// Objeto de transferência de dados (DTO) que define os parâmetros obrigatórios para a criação ou atualização de um Torneio.
/// </summary>
public class TournamentCreateDTO
{
    /// <summary>
    /// Nome descritivo do torneio a ser inserido ou modificado no sistema.
    /// </summary>
    [Required(ErrorMessage = "O nome do torneio é de preenchimento obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do torneio não pode exceder os {1} caracteres.")]
    public string Name { get; set; } = "";
}