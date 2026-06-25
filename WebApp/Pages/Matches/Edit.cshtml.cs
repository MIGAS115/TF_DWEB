using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Matches;

/// <summary>
/// PageModel responsável por processar a edição e modificação de um Jogo existente.
/// Restrito a utilizadores com os cargos Admin ou Gestor, validando a posse do recurso (Ownership).
/// </summary>
[Authorize(Roles = "Admin,Gestor")]
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
    /// Propriedade vinculada ao formulário que contém os dados do jogo a ser editado.
    /// </summary>
    [BindProperty]
    public Match Match { get; set; } = default!;

    /// <summary>
    /// Carrega os dados do jogo especificado e inicializa as listas de seleção, 
    /// validando se o utilizador autenticado tem permissão para o editar.
    /// </summary>
    /// <param name="id">Identificador numérico do jogo.</param>
    /// <returns>A página preenchida ou NotFound/Forbid em caso de erro/proibição.</returns>
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

        // Validação de Ownership
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && match.OwnerId != loggedInUserId)
        {
            return Forbid();
        }

        Match = match;

        CarregarDadosDropdown();
        return Page();
    }

    /// <summary>
    /// Processa a submissão do formulário, valida regras de negócio, verifica propriedade e grava as alterações.
    /// </summary>
    /// <returns>Redirecionamento para o Index ou a mesma página com validações de erro.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            CarregarDadosDropdown();
            return Page();
        }

        var matchToUpdate = await _context.Matches.FirstOrDefaultAsync(m => m.Id == Match.Id);
        if (matchToUpdate == null)
        {
            return NotFound();
        }

        // Validação de Ownership (Segurança contra POST falsificado)
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && matchToUpdate.OwnerId != loggedInUserId)
        {
            return Forbid();
        }

        if (Match.HomeTeamFK == Match.AwayTeamFK)
        {
            ModelState.AddModelError(string.Empty, "A equipa da casa não pode ser a mesma que a equipa visitante.");
            CarregarDadosDropdown();
            return Page();
        }

        // Atualização dos campos permitidos
        matchToUpdate.HomeTeamFK = Match.HomeTeamFK;
        matchToUpdate.AwayTeamFK = Match.AwayTeamFK;
        matchToUpdate.MatchDate = Match.MatchDate;
        matchToUpdate.TournamentFK = Match.TournamentFK;
        matchToUpdate.IsManualOverride = true;

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MatchExists(Match.Id))
            {
                return NotFound();
            }
            throw;
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno ao atualizar o jogo. Contacte o administrador.");
        }
    }

    /// <summary>
    /// Auxiliar para carregar os dados das Dropdowns.
    /// </summary>
    private void CarregarDadosDropdown()
    {
        ViewData["HomeTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
        ViewData["AwayTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
        ViewData["TournamentFK"] = new SelectList(_context.Tournaments, "Id", "Name");
    }

    private bool MatchExists(int id)
    {
        return (_context.Matches?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}