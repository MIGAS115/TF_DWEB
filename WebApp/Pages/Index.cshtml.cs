using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages;

/// <summary>
/// Modelo de página para o Dashboard inicial (Home).
/// Gerencia a listagem de jogos aplicando regras de paginação e usabilidade.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="IndexModel"/>.
    /// </summary>
    /// <param name="context">O contexto de base de dados da aplicação.</param>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtém ou define a coleção de jogos correspondente à página atual.
    /// </summary>
    public IList<Match> ExistingMatches { get; set; } = [];

    /// <summary>
    /// Obtém ou define o índice da página corrente na interface.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public int PaginaAtual { get; set; } = 1;

    /// <summary>
    /// Obtém ou define o total de páginas disponíveis com base nos registos existentes.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET inicial carregando os jogos paginados.
    /// </summary>
    /// <param name="paginaAtual">Parâmetro opcional para navegar entre páginas.</param>
    public async Task OnGetAsync(int? paginaAtual)
    {
        if (paginaAtual.HasValue && paginaAtual.Value > 0)
        {
            PaginaAtual = paginaAtual.Value;
        }

        // Define quantos cartões queres mostrar por ecrã (ex: 9 ou 12 para grelhas de 3 colunas)
        int tamanhoPagina = 9;

        // Conta o total de registos para calcular as páginas necessárias
        int totalRegistos = await _context.Matches.CountAsync();
        TotalPaginas = (int)Math.Ceiling(totalRegistos / (double)tamanhoPagina);

        // Garante que o utilizador não tenta aceder a páginas fora dos limites
        if (PaginaAtual > TotalPaginas && TotalPaginas > 0)
        {
            PaginaAtual = TotalPaginas;
        }

        // Aplica Skip e Take de forma eficiente na BD via EF Core e LINQ
        ExistingMatches = await _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Tournament)
            .OrderByDescending(m => m.MatchDate)
            .Skip((PaginaAtual - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .AsNoTracking()
            .ToListAsync();
    }
}