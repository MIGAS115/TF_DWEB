using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApp.Pages.Matches;

/// <summary>
/// PageModel responsável por listar todos os jogos (matches) agendados na plataforma com suporte a segmentação paginada.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de base de dados.
    /// </summary>
    /// <param name="context">O contexto da BD partilhado.</param>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista de jogos a ser renderizada na página Razor.
    /// </summary>
    public IList<Match> Match { get; set; } = [];

    /// <summary>
    /// Conjunto de IDs de jogos que o utilizador autenticado tem permissão para modificar.
    /// </summary>
    public HashSet<int> EditableMatchIds { get; set; } = new HashSet<int>();

    /// <summary>
    /// Método executado ao carregar a página, responsável por ir buscar os jogos à base de dados.
    /// Obtém ou define o índice da página corrente na interface gráfica.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public int PaginaAtual { get; set; } = 1;

    /// <summary>
    /// Obtém ou define o total de páginas disponíveis com base nos jogos existentes.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Método executado ao carregar a página, responsável por ir buscar os jogos à base de dados de forma paginada.
    /// </summary>
    /// <param name="paginaAtual">Parâmetro opcional correspondente à página solicitada.</param>
    /// <returns>Uma Task representando a operação assíncrona.</returns>
    public async Task OnGetAsync(int? paginaAtual)
    {
        if (paginaAtual.HasValue && paginaAtual.Value > 0)
        {
            PaginaAtual = paginaAtual.Value;
        }

        int tamanhoPagina = 9;

        if (_context.Matches != null)
        {
            int totalRegistos = await _context.Matches.CountAsync();
            TotalPaginas = (int)Math.Ceiling(totalRegistos / (double)tamanhoPagina);

            if (PaginaAtual > TotalPaginas && TotalPaginas > 0)
            {
                PaginaAtual = TotalPaginas;
            }

            Match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Tournament)
                .OrderByDescending(m => m.MatchDate)
                .Skip((PaginaAtual - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .AsNoTracking()
                .ToListAsync();

            // Lógica de permissões para UI
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var isGestor = User.IsInRole("Gestor");

            foreach (var match in Match)
            {
                // Permissão: Admin ou o Gestor que criou o registo (Ownership)
                if (isAdmin || (isGestor && !string.IsNullOrEmpty(loggedInUserId) && match.OwnerId == loggedInUserId))
                {
                    EditableMatchIds.Add(match.Id);
                }
            }
        }
    }
}