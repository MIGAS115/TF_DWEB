using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

/// <summary>
/// Modelo de página com informações institucionais, autores e referências bibliográficas.
/// Cumpre o requisito obrigatório de identificação de código de terceiros e credenciais de acesso.
/// </summary>
public class SobreModel : PageModel
{
    /// <summary>
    /// Processa o pedido HTTP GET para a página informativa do projeto.
    /// </summary>
    public void OnGet()
    {
    }
}