using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Tournaments;

/// <summary>
/// PageModel responsável pela interface de validação relacional e deleção de torneios.
/// Restrito a utilizadores com os cargos Admin ou Gestor, respeitando a propriedade do recurso (Ownership).
/// </summary>
[Authorize(Roles = "Admin,Gestor")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de base de dados.
    /// </summary>
    /// <param name="context">O contexto de dados partilhado.</param>
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Propriedade vinculada que expõe o torneio pendente de eliminação.
    /// </summary>
    [BindProperty]
    public Tournament Tournament { get; set; } = default!;

    /// <summary>
    /// Recupera os dados do torneio para confirmação de exclusão pelo utilizador.
    /// Valida se o utilizador autenticado tem permissão para o eliminar.
    /// </summary>
    /// <param name="id">Identificador do torneio.</param>
    /// <returns>A página de confirmação ou NotFound/Forbid.</returns>
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

        // --- VALIDAÇÃO DE OWNERSHIP ---
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && tournament.OwnerId != loggedInUserId)
        {
            return Forbid();
        }

        Tournament = tournament;
        return Page();
    }

    /// <summary>
    /// Avalia a integridade restritiva, valida permissões e processa a remoção do torneio se não contiver dependências.
    /// </summary>
    /// <param name="id">Identificador do torneio a remover.</param>
    /// <returns>Redirecionamento em caso de sucesso, página corrente com avisos ou Erro 500.</returns>
    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.Tournaments == null)
        {
            return NotFound();
        }

        var tournament = await _context.Tournaments.FindAsync(id);
        if (tournament == null)
        {
            return NotFound();
        }

        // Validação de Ownership (Segurança contra POST falsificado)
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && tournament.OwnerId != loggedInUserId)
        {
            return Forbid();
        }

        Tournament = tournament;

        var temJogosVinculados = await _context.Matches.AnyAsync(m => m.TournamentFK == id);
        var temEquipasVinculadas = await _context.TournamentTeams.AnyAsync(tt => tt.TournamentFK == id);

        if (temJogosVinculados || temEquipasVinculadas)
        {
            ModelState.AddModelError(string.Empty, "Não é possível remover este torneio porque existem jogos agendados ou equipas inscritas associadas ao mesmo.");
            return Page();
        }

        try
        {
            _context.Tournaments.Remove(Tournament);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno ao tentar eliminar o torneio. Contacte o administrador.");
        }
    }
}