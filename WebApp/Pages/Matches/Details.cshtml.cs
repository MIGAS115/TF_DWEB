using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Matches;

/// <summary>
/// PageModel responsável por carregar e apresentar os detalhes completos de um jogo específico.
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
    /// Armazena os dados detalhados do jogo consultado para renderização na interface.
    /// </summary>
    public Match Match { get; set; } = default!;

    /// <summary>
    /// Processa a requisição de leitura do recurso e carrega as propriedades de navegação associadas.
    /// </summary>
    /// <param name="id">O identificador numérico único do jogo.</param>
    /// <returns>A página preenchida com as informações do jogo ou NotFound caso não exista.</returns>
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

        Match = match;
        return Page();
    }
}