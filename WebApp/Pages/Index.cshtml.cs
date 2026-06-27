using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Pages;

/// <summary>
/// Modelo de página para o Dashboard inicial (Home).
/// Gerencia a listagem de jogos aplicando regras de paginação, filtragem e usabilidade.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="IndexModel"/>.
    /// </summary>
    /// <param name="context">O contexto de base de dados da aplicação.</param>
    public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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
    /// Obtém ou define o filtro de estado atual para a listagem (ex: running, not_started, finished).
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? FiltroEstado { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? FiltroCategoria { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool ApenasFavoritos { get; set; }

    /// <summary>
    /// Obtém ou define o total de páginas disponíveis com base nos registos existentes e filtrados.
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Processa o pedido HTTP GET inicial carregando os jogos paginados e filtrados.
    /// </summary>
    /// <param name="paginaAtual">Parâmetro opcional para navegar entre páginas.</param>
    /// <param name="filtroEstado">Parâmetro opcional para filtrar os jogos pelo estado (PandaScore).</param>
    public async Task OnGetAsync(int? paginaAtual, string? filtroEstado,string? filtroCategoria, bool apenasFavoritos)
    {
        if (paginaAtual.HasValue && paginaAtual.Value > 0)
        {
            PaginaAtual = paginaAtual.Value;

        }

        FiltroEstado = filtroEstado;
        FiltroCategoria = filtroCategoria;
        ApenasFavoritos = apenasFavoritos;

        // Define quantos cartões queres mostrar por ecrã (ex: 9 ou 12 para grelhas de 3 colunas)
        int tamanhoPagina = 9;

        // Inicia a construção da query base (IQueryable permite adiar a execução na BD)
        IQueryable<Match> query = _context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Tournament);
 

        // Aplica o filtro de estado se existir seleção por parte do utilizador
        if (!string.IsNullOrEmpty(FiltroEstado))
        {
            query = query.Where(m => m.Status.ToUpper() == FiltroEstado.ToUpper());
        }
        // Aplica filtro de categoria
        if (!string.IsNullOrEmpty(FiltroCategoria))
        {
            string filtro = FiltroCategoria.ToLower().Trim();
            query = query.Where(m => m.Tournament.GameName.ToLower() == filtro);
        }

        // Aplica filtro de favoritos (Assumindo que tem a tabela Favorites ligada ao utilizador)
        if (ApenasFavoritos && User.Identity.IsAuthenticated)
        {
            var userId = _userManager.GetUserId(User);
            query = query.Where(m => _context.Favorites
                .Any(f => f.NormalFK.Equals(userId) && (f.TeamFK == m.HomeTeamFK || f.TeamFK == m.AwayTeamFK)));
        }
        // Conta o total de registos FILTRADOS para calcular as páginas necessárias
        int totalRegistos = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(totalRegistos / (double)tamanhoPagina);

        // Garante que o utilizador não tenta aceder a páginas fora dos limites
        if (PaginaAtual > TotalPaginas && TotalPaginas > 0)
        {
            PaginaAtual = TotalPaginas;
        }

        // Aplica ordenação, Skip e Take de forma eficiente na BD via EF Core e LINQ
        ExistingMatches = await query
            .OrderByDescending(m => m.MatchDate)
            .Skip((PaginaAtual - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .AsNoTracking()
            .ToListAsync();
    }
}