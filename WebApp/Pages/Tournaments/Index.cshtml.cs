using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Tournaments;

/// <summary>
/// PageModel responsável por listar todos os torneios registados na plataforma através de paginação.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor com injeção de dependência do contexto de base de dados.
    /// </summary>
    /// <param name="context">O contexto de dados partilhado.</param>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Coleção de torneios a ser renderizada na interface gráfica da página.
    /// </summary>
    public IList<Tournament> Tournaments { get; set; } = [];

    /// <summary>
    /// Obtém ou define o índice da página corrente na interface gráfica.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public int PaginaAtual { get; set; } = 1;

    /// <summary>
    /// Obtém ou define o total de páginas disponíveis com base nos torneios existentes.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Executa a leitura assíncrona, ordenada e paginada dos torneios registados na base de dados.
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

        if (_context.Tournaments != null)
        {
            int totalRegistos = await _context.Tournaments.CountAsync();
            TotalPaginas = (int)Math.Ceiling(totalRegistos / (double)tamanhoPagina);

            if (PaginaAtual > TotalPaginas && TotalPaginas > 0)
            {
                PaginaAtual = TotalPaginas;
            }

            Tournaments = await _context.Tournaments
                .OrderBy(t => t.Name)
                .Skip((PaginaAtual - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}