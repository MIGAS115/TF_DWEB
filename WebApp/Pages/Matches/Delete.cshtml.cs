using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Matches;

/// <summary>
/// PageModel responsável pela apresentação dos detalhes e remoção definitiva de um jogo.
/// Restrito a utilizadores com cargos Admin ou Gestor, validando a posse do recurso (Ownership).
/// </summary>
[Authorize(Roles = "Admin,Gestor")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de dados.
    /// </summary>
    /// <param name="context">O contexto da BD partilhado.</param>
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Propriedade vinculada que armazena os dados do jogo a eliminar para visualização na interface.
    /// </summary>
    [BindProperty]
    public Match Match { get; set; } = default!;

    /// <summary>
    /// Carrega os dados detalhados do jogo para confirmação visual de eliminação, 
    /// validando se o utilizador autenticado tem permissão para aceder a este recurso.
    /// </summary>
    /// <param name="id">Identificador do jogo.</param>
    /// <returns>A página preenchida ou Forbid/NotFound em caso de erro.</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Matches == null)
        {
            return NotFound();
        }

        var match = await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Tournament)
            .FirstOrDefaultAsync(m => m.Id == id);

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
        return Page();
    }

    /// <summary>
    /// Processa o pedido de remoção física do jogo da base de dados, garantindo permissões.
    /// </summary>
    /// <param name="id">Identificador do jogo a remover.</param>
    /// <returns>Redirecionamento para a listagem principal ou erro 500.</returns>
    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.Matches == null)
        {
            return NotFound();
        }

        var match = await _context.Matches.FindAsync(id);

        if (match == null)
        {
            return NotFound();
        }

        // Validação de Ownership (Segurança contra POST falsificado)
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && match.OwnerId != loggedInUserId)
        {
            return Forbid();
        }

        try
        {
            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno ao tentar eliminar o jogo. Contacte o administrador.");
        }
    }
}