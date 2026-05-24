using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Tournaments;

/// <summary>
/// PageModel responsável por processar a criação de novos torneios no sistema.
/// </summary>
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de base de dados.
    /// </summary>
    /// <param name="context">O contexto de dados partilhado.</param>
    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Disponibiliza o formulário de criação de torneios.
    /// </summary>
    /// <returns>O resultado de renderização da página.</returns>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>
    /// Propriedade vinculada ao formulário que captura os dados do novo torneio.
    /// </summary>
    [BindProperty]
    public Tournament Tournament { get; set; } = default!;

    /// <summary>
    /// Valida e persiste o novo torneio na base de dados.
    /// </summary>
    /// <returns>Redirecionamento para a listagem ou recarregamento em caso de erro.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.Tournaments == null || Tournament == null)
        {
            return Page();
        }

        Tournament.IsManualOverride = true;

        _context.Tournaments.Add(Tournament);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}