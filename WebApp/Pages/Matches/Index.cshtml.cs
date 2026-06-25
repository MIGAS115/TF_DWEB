using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
    /// Conjunto de IDs de jogos que o utilizador autenticado tem permissão para modificar.
    /// </summary>
    public HashSet<int> EditableMatchIds { get; set; } = new HashSet<int>();

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

            // Lógica de permissões para UI
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var isGestor = User.IsInRole("Gestor");

            foreach (var match in Match)
            {
                // Permissão: Admin ou o Gestor que criou o registo (Ownership)
                if (isAdmin || (isGestor && !string.IsNullOrEmpty(loggedInUserId) && match.OwnerId == loggedInUserId))
                {
                    EditableMatchIds.Add(match.Id);
                }
            }
        }
    }
}