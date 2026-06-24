using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

/// <summary>
/// Modelo de página para o Dashboard inicial (Home).
/// Responsável por gerir a listagem principal de jogos de e-sports.
/// </summary>
public class IndexModel : PageModel
{
    /// <summary>
    /// Processa o pedido HTTP GET inicial para a página principal.
    /// Utilizado para carregar o estado inicial dos jogos diretamente da base de dados antes de o SignalR assumir o controlo em tempo real.
    /// </summary>
    public void OnGet()
    {
    }
}