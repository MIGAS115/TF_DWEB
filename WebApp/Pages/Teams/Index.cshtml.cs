using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams;

/// <summary>
/// PageModel responsável pela listagem de equipas com suporte a paginação e gestão do estado de favoritos dos utilizadores.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<MyUser> _userManager;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="IndexModel"/> com injeção de dependências.
    /// </summary>
    /// <param name="context">O contexto da base de dados global.</param>
    /// <param name="userManager">O gestor de identidades de utilizadores do ASP.NET Identity.</param>
    public IndexModel(ApplicationDbContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Coleção de equipas a exibir na listagem correspondente à página selecionada.
    /// </summary>
    public IList<Team> Teams { get; set; } = [];

    /// <summary>
    /// Lista de IDs das equipas favoritas do utilizador atualmente autenticado.
    /// </summary>
    public List<int> UserFavoriteTeamIds { get; set; } = [];

    /// <summary>
    /// Obtém ou define o índice da página corrente na interface gráfica.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public int PaginaAtual { get; set; } = 1;

    /// <summary>
    /// Obtém ou define o total de páginas disponíveis com base nos registos existentes de equipas.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Executa o carregamento assíncrono e paginado das equipas registadas e mapeia os favoritos do utilizador logado.
    /// </summary>
    /// <param name="paginaAtual">Parâmetro opcional para saltar para uma página de registos específica.</param>
    /// <returns>Uma Task representando a operação assíncrona.</returns>
    public async Task OnGetAsync(int? paginaAtual)
    {
        if (paginaAtual.HasValue && paginaAtual.Value > 0)
        {
            PaginaAtual = paginaAtual.Value;
        }

        int tamanhoPagina = 9;

        if (_context.Teams != null)
        {
            int totalRegistos = await _context.Teams.CountAsync();
            TotalPaginas = (int)Math.Ceiling(totalRegistos / (double)tamanhoPagina);

            if (PaginaAtual > TotalPaginas && TotalPaginas > 0)
            {
                PaginaAtual = TotalPaginas;
            }

            Teams = await _context.Teams
                .OrderBy(t => t.Name)
                .Skip((PaginaAtual - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .AsNoTracking()
                .ToListAsync();
        }

        var user = await _userManager.GetUserAsync(User);

        if (user != null && _context.Favorites != null)
        {
            UserFavoriteTeamIds = await _context.Favorites
                .Where(f => f.NormalFK == user.Id)
                .Select(f => f.TeamFK)
                .ToListAsync();
        }
    }

    /// <summary>
    /// Alterna o estado de favorito de uma equipa para o utilizador autenticado.
    /// </summary>
    /// <param name="teamId">ID da equipa selecionada.</param>
    /// <returns>Recarrega a página atual com o estado atualizado de favoritos.</returns>
    public async Task<IActionResult> OnPostToggleFavoriteAsync(int teamId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var teamExists = await _context.Teams.AnyAsync(t => t.Id == teamId);
        if (!teamExists)
        {
            return NotFound();
        }

        var isRegularUser = await _context.RegularUsers.AnyAsync(n => n.Id == user.Id);

        if (!isRegularUser)
        {
            return BadRequest("Perfil de utilizador não configurado ou o utilizador atual é um Administrador.");
        }

        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.NormalFK == user.Id && f.TeamFK == teamId);

        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
        }
        else
        {
            var newFavorite = new Favorite
            {
                NormalFK = user.Id,
                TeamFK = teamId
            };
            _context.Favorites.Add(newFavorite);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            _context.ChangeTracker.Clear();
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro interno ao processar a operação de favoritos.");
        }

        return RedirectToPage("./Index", new { paginaAtual = PaginaAtual });
    }
}