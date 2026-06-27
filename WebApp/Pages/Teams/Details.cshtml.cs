using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams;

/// <summary>
/// PageModel responsável por carregar os detalhes de uma equipa, incluindo estatísticas de jogos e categorias.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de dados.
    /// </summary>
    /// <param name="context">O contexto de base de dados da aplicação.</param>
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Propriedade que contém os dados da equipa a exibir.
    /// </summary>
    public Team Team { get; set; } = default!;

    /// <summary>
    /// Quantidade total de jogos realizados pela equipa (Casa + Fora).
    /// </summary>
    public int TotalMatches { get; set; }

    /// <summary>
    /// Carrega os dados da equipa, incluindo a categoria associada e o total de jogos.
    /// </summary>
    /// <param name="id">O identificador da equipa.</param>
    /// <returns>A página de detalhes ou NotFound se não existir.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Teams
            .Include(t => t.Category)
            .Include(t => t.HomeMatches)
            .Include(t => t.AwayMatches)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (team == null)
        {
            return NotFound();
        }

        Team = team;
        TotalMatches = team.HomeMatches.Count + team.AwayMatches.Count;

        return Page();
    }
}