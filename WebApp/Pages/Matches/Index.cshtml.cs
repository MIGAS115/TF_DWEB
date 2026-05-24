using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Matches;

/// <summary>
/// PageModel responsável por listar todos os jogos (matches) agendados na plataforma.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de base de dados.
    /// </summary>
    /// <param name="context">O contexto da BD partilhado.</param>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista de jogos a ser renderizada na página Razor.
    /// </summary>
    public IList<Match> Match { get; set; } = default!;

    /// <summary>
    /// Método executado ao carregar a página, responsável por ir buscar os jogos à base de dados.
    /// </summary>
    public async Task OnGetAsync()
    {
        if (_context.Matches != null)
        {
            Match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Tournament)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();
        }
    }
}