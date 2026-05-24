using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Tournaments;

/// <summary>
/// PageModel responsável por expor os detalhes informativos de um torneio específico.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de base de dados.
    /// </summary>
    /// <param name="context">O contexto de dados partilhado.</param>
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Armazena a entidade de torneio localizada para apresentação visual.
    /// </summary>
    public Tournament Tournament { get; set; } = default!;

    /// <summary>
    /// Executa a busca detalhada do torneio com base no identificador.
    /// </summary>
    /// <param name="id">Identificador do torneio.</param>
    /// <returns>A página de detalhes ou NotFound.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Tournaments == null)
        {
            return NotFound();
        }

        var tournament = await _context.Tournaments.FirstOrDefaultAsync(m => m.Id == id);
        if (tournament == null)
        {
            return NotFound();
        }

        Tournament = tournament;
        return Page();
    }
}