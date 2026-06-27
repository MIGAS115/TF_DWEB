using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Globalization;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Tournaments;

/// <summary>
/// PageModel responsável por gerir a modificação de dados de um torneio existente.
/// Restrito a utilizadores com os cargos Admin ou Gestor, respeitando a propriedade do recurso (Ownership).
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
    /// Propriedade vinculada que armazena os dados do torneio a ser editado.
    /// </summary>
    [BindProperty]
    public Tournament Tournament { get; set; } = default!;

    /// <summary>
    /// Carrega as informações do torneio, validando se o utilizador autenticado tem permissão para o editar.
    /// </summary>
    /// <param name="id">Identificador único do torneio.</param>
    /// <returns>A página com os dados preenchidos ou Forbid/NotFound.</returns>
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

        // Validação de Ownership
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && tournament.OwnerId != loggedInUserId)
        {
            return Forbid(); // Retorna o Erro HTTP 403 (Acesso Negado)
        }

        Tournament = tournament;

        // Prepara o campo Auxiliar para exibir a vírgula corretamente no HTML
        if (Tournament.PrizePool.HasValue)
        {
            Tournament.PrizePoolAux = Tournament.PrizePool.Value.ToString("0.00", new CultureInfo("pt-PT"));
        }

        return Page();
    }

    /// <summary>
    /// Processa, valida e grava as modificações, garantindo que a segurança não foi contornada via POST.
    /// </summary>
    /// <returns>Redirecionamento para o Index ou a página corrente com erros, ou 500 em falha crítica.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // Tratamento da propriedade auxiliar de decimais (Padrão pt-PT)
        if (!string.IsNullOrWhiteSpace(Tournament.PrizePoolAux))
        {
            try
            {
                Tournament.PrizePool = Convert.ToDecimal(
                    Tournament.PrizePoolAux.Replace('.', ','),
                    new CultureInfo("pt-PT")
                );
            }
            catch
            {
                ModelState.AddModelError("Tournament.PrizePoolAux", "O valor introduzido para o Prémio Total é inválido.");
            }
        }
        else
        {
            Tournament.PrizePool = null;
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var tournamentToUpdate = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == Tournament.Id);
        if (tournamentToUpdate == null)
        {
            return NotFound();
        }

        // Validação de Ownership (Segurança contra POST falsificado)
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && tournamentToUpdate.OwnerId != loggedInUserId)
        {
            return Forbid();
        }

        // Atualizar campos permitidos
        tournamentToUpdate.Name = Tournament.Name;
        tournamentToUpdate.GameName = Tournament.GameName;
        tournamentToUpdate.PrizePool = Tournament.PrizePool;
        // O OwnerId não é atualizado para garantir que o dono original se mantém

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
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
        catch (Exception)
        {
            // Ocultação da Stack Trace em produção
            return StatusCode(500, "Erro interno ao atualizar torneio. Contacte o administrador.");
        }
    }

    private bool TournamentExists(int id)
    {
        return (_context.Tournaments?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}