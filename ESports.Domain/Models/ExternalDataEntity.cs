using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models;

/// <summary>
/// Define as propriedades base para suporte a dados integrados via API externa
/// ou criados/editados manualmente através do painel de administração.
/// </summary>
public abstract class ExternalDataEntity
{
    /// <summary>
    /// Obtém ou define um valor que indica se o registo foi alterado ou introduzido
    /// manualmente, bloqueando sobreposições automáticas da API externa.
    /// </summary>
    [Display(Name = "Substituição Manual")]
    public bool IsManualOverride { get; set; }

    /// <summary>
    /// Obtém ou define o identificador único do registo proveniente da API externa.
    /// Mantém-se nulo se o registo for estritamente local.
    /// </summary>
    [Display(Name = "ID da Fonte Externa")]
    public string? ExternalSourceId { get; set; }
}