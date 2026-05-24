using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Matches;

/// <summary>
/// PageModel responsável por processar a edição e modificação de um Jogo existente.
/// </summary>
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto da base de dados.
    /// </summary>
    /// <param name="context">O contexto de dados partilhado.</param>
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Propriedade vinculada ao formulário que contém os dados do jogo a ser editado.
    /// </summary>
    [BindProperty]
    public Match Match { get; set; } = default!;

    /// <summary>
    /// Carrega os dados do jogo especificado e inicializa as listas de seleção.
    /// </summary>
    /// <param name="id">Identificador numérico do jogo.</param>
    /// <returns>A página preenchida ou NotFound em caso de erro.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Matches == null)
        {
            return NotFound();
        }

        var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);
        if (match == null)
        {
            return NotFound();
        }

        Match = match;

        ViewData["HomeTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
        ViewData["AwayTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
        ViewData["TournamentFK"] = new SelectList(_context.Tournaments, "Id", "Name");

        return Page();
    }

    /// <summary>
    /// Processa a submissão do formulário, valida regras de negócio e grava as alterações.
    /// </summary>
    /// <returns>Redirecionamento para o Index ou a mesma página com validações de erro.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ViewData["HomeTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
            ViewData["AwayTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
            ViewData["TournamentFK"] = new SelectList(_context.Tournaments, "Id", "Name");
            return Page();
        }

        if (Match.HomeTeamFK == Match.AwayTeamFK)
        {
            ModelState.AddModelError(string.Empty, "A equipa da casa não pode ser a mesma que a equipa visitante.");
            ViewData["HomeTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
            ViewData["AwayTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
            ViewData["TournamentFK"] = new SelectList(_context.Tournaments, "Id", "Name");
            return Page();
        }

        var matchToUpdate = await _context.Matches.FirstOrDefaultAsync(m => m.Id == Match.Id);
        if (matchToUpdate == null)
        {
            return NotFound();
        }

        matchToUpdate.HomeTeamFK = Match.HomeTeamFK;
        matchToUpdate.AwayTeamFK = Match.AwayTeamFK;
        matchToUpdate.MatchDate = Match.MatchDate;
        matchToUpdate.TournamentFK = Match.TournamentFK;
        matchToUpdate.IsManualOverride = true;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MatchExists(Match.Id))
            {
                return NotFound();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "O jogo foi modificado por outro utilizador. Por favor, recarregue a página.");
                ViewData["HomeTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
                ViewData["AwayTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
                ViewData["TournamentFK"] = new SelectList(_context.Tournaments, "Id", "Name");
                return Page();
            }
        }

        return RedirectToPage("./Index");
    }

    /// <summary>
    /// Verifica a existência física do jogo na base de dados.
    /// </summary>
    /// <param name="id">Identificador do jogo.</param>
    /// <returns>Verdadeiro se existir, falso caso contrário.</returns>
    private bool MatchExists(int id)
    {
        return (_context.Matches?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}