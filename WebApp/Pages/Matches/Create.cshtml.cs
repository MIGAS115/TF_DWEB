using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Matches;

/// <summary>
/// PageModel responsável por fornecer os dados e processar o formulário de criação de um novo Jogo (Match).
/// </summary>
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de base de dados.
    /// </summary>
    /// <param name="context">O contexto da BD partilhado.</param>
    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Método acionado ao carregar a página para inicializar as caixas de seleção (Dropdowns) de Equipas e Torneios.
    /// </summary>
    /// <returns>A página Razor correspondente.</returns>
    public IActionResult OnGet()
    {
        ViewData["HomeTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
        ViewData["AwayTeamFK"] = new SelectList(_context.Teams, "Id", "Name");
        ViewData["TournamentFK"] = new SelectList(_context.Tournaments, "Id", "Name");

        return Page();
    }

    /// <summary>
    /// Propriedade vinculada bidirecionalmente que recebe os dados do jogo submetidos pelo utilizador.
    /// </summary>
    [BindProperty]
    public Match Match { get; set; } = default!;

    /// <summary>
    /// Método acionado pela submissão do formulário (POST) para validar regras de negócio e gravar o jogo.
    /// </summary>
    /// <returns>Redirecionamento para o Index em caso de sucesso ou recarregamento com mensagens de erro.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.Matches == null || Match == null)
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

        Match.IsManualOverride = true;

        _context.Matches.Add(Match);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}