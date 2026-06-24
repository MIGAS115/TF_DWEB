using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Tournaments;

/// <summary>
/// PageModel responsável por listar todos os torneios registados na plataforma.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de base de dados.
    /// </summary>
    /// <param name="context">O contexto de dados partilhado.</param>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Coleção de torneios a ser renderizada na interface gráfica da página.
    /// </summary>
    public IList<Tournament> Tournaments { get; set; } = [];

    /// <summary>
    /// Executa a leitura assíncrona dos torneios ordenados alfabeticamente.
    /// </summary>
    public async Task OnGetAsync()
    {
        if (_context.Tournaments != null)
        {
            Tournaments = await _context.Tournaments
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
    }
}