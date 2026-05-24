using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Tournaments;

/// <summary>
/// PageModel responsável por gerir a modificação de dados de um torneio existente.
/// </summary>
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de base de dados.
    /// </summary>
    /// <param name="context">O contexto de dados partilhado.</param>
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Propriedade vinculada que armazena os dados do torneio a ser editado.
    /// </summary>
    [BindProperty]
    public Tournament Tournament { get; set; } = default!;

    /// <summary>
    /// Carrega as informações do torneio correspondente ao identificador fornecido.
    /// </summary>
    /// <param name="id">Identificador único do torneio.</param>
    /// <returns>A página com os dados preenchidos ou NotFound.</returns>
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

    /// <summary>
    /// Processa e valida as modificações submetidas controlando falhas de concorrência.
    /// </summary>
    /// <returns>Redirecionamento para o Index ou a página corrente com erros.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var tournamentToUpdate = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == Tournament.Id);
        if (tournamentToUpdate == null)
        {
            return NotFound();
        }

        tournamentToUpdate.Name = Tournament.Name;
        tournamentToUpdate.GameName = Tournament.GameName;
        tournamentToUpdate.IsManualOverride = true;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TournamentExists(Tournament.Id))
            {
                return NotFound();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "O torneio foi modificado por outro utilizador. Por favor, recarregue a página.");
                return Page();
            }
        }

        return RedirectToPage("./Index");
    }

    /// <summary>
    /// Avalia a existência física do registo de torneio.
    /// </summary>
    /// <param name="id">Identificador do torneio.</param>
    /// <returns>Verdadeiro se o registo existir.</returns>
    private bool TournamentExists(int id)
    {
        return (_context.Tournaments?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}