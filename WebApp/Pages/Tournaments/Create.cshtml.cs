using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ESports.Domain.Data;
using ESports.Domain.Models;
using System.Security.Claims;
using System.Globalization;

namespace WebApp.Pages.Tournaments;

/// <summary>
/// PageModel responsável por processar a criação de novos torneios no sistema.
/// Restrito a utilizadores com o cargo de Admin ou Gestor.
/// </summary>
[Authorize(Roles = "Admin,Gestor")]
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
    /// Valida, sanitiza os decimais e persiste o novo torneio na base de dados, associando-o ao utilizador logado.
    /// </summary>
    /// <returns>Redirecionamento para a listagem ou mensagem de erro 500 em caso de falha.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        // 1. Validar e converter o PrizePoolAux (Padrão de Localização pt-PT)
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

        // 2. Validação padrão do estado do modelo
        if (!ModelState.IsValid || _context.Tournaments == null || Tournament == null)
        {
            return Page();
        }

        // 3. Associar as métricas de controlo e ownership
        Tournament.IsManualOverride = true;

        // Captura o ID do utilizador autenticado e associa-o como Dono do Torneio
        // (Nota: Garante que a tua classe Tournament em ESports.Domain.Models tem a propriedade public string? OwnerId { get; set; })
        Tournament.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 4. Gravação segura com tratamento de erros limpo
        try
        {
            _context.Tournaments.Add(Tournament);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (Exception)
        {
            // Oculta a stack trace em produção conforme as regras
            return StatusCode(500, "Erro interno ao criar torneio. Contacte o administrador.");
        }
    }
}