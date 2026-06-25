using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
    /// Lista de IDs de torneios que o utilizador autenticado tem permissão para modificar (Editar/Eliminar).
    /// </summary>
    public HashSet<int> EditableTournamentIds { get; set; } = new HashSet<int>();

    /// <summary>
    /// Executa a leitura assíncrona dos torneios ordenados alfabeticamente e filtra as permissões de edição.
    /// </summary>
    public async Task OnGetAsync()
    {
        if (_context.Tournaments != null)
        {
            Tournaments = await _context.Tournaments
                .OrderBy(t => t.Name)
                .ToListAsync();

            // Identificar o utilizador logado e os seus privilégios
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            foreach (var tournament in Tournaments)
            {
                // Regra de Ownership: O Admin vê tudo, o Gestor vê apenas os que criou
                if (isAdmin || (!string.IsNullOrEmpty(loggedInUserId) && tournament.OwnerId == loggedInUserId))
                {
                    EditableTournamentIds.Add(tournament.Id);
                }
            }
        }
    }
}