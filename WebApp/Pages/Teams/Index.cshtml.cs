using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    /// <summary>
    /// PageModel responsável pela listagem de equipas, paginação de resultados e gestão do estado de favoritos dos utilizadores.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="IndexModel"/> com injeção de dependências.
        /// </summary>
        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Lista das equipas a apresentar na página atual.
        /// </summary>
        public IList<Team> Teams { get; set; } = [];

        /// <summary>
        /// Lista de IDs de equipas marcadas como favoritas pelo utilizador autenticado.
        /// </summary>
        public List<int> UserFavoriteTeamIds { get; set; } = [];

        /// <summary>
        /// Lista de IDs de equipas que o utilizador autenticado tem permissão para modificar (Editar/Eliminar).
        /// </summary>
        public HashSet<int> EditableTeamIds { get; set; } = new HashSet<int>();

        /// <summary>
        /// Página atual selecionada na paginação, capturada via QueryString (GET).
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int PaginaAtual { get; set; } = 1;

        /// <summary>
        /// Número total de páginas calculado com base na quantidade de registos e limite por página.
        /// </summary>
        public int TotalPaginas { get; set; }

        /// <summary>
        /// Executa o carregamento assíncrono das equipas registadas, aplicando limites de paginação e validando permissões.
        /// </summary>
        public async Task OnGetAsync()
        {
            if (_context.Teams == null)
            {
                return;
            }

            // --- Lógica de Paginação ---
            int pageSize = 10; // Número de itens por página (Ajustável)
            var totalTeams = await _context.Teams.CountAsync();
            TotalPaginas = (int)Math.Ceiling(totalTeams / (double)pageSize);

            // Validação de segurança para limites da página
            if (PaginaAtual < 1) PaginaAtual = 1;
            if (PaginaAtual > TotalPaginas && TotalPaginas > 0) PaginaAtual = TotalPaginas;

            // Busca otimizada usando EF Core com Skip/Take
            Teams = await _context.Teams
                .Skip((PaginaAtual - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // --- Lógica de Ownership para a interface ---
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            foreach (var team in Teams)
            {
                // Se é admin ou é o dono do registo, adiciona ao HashSet de itens editáveis
                if (isAdmin || (!string.IsNullOrEmpty(loggedInUserId) && team.OwnerId == loggedInUserId))
                {
                    EditableTeamIds.Add(team.Id);
                }
            }

            // --- Obtém a credencial de segurança autenticada para Favoritos ---
            var identityUser = await _userManager.GetUserAsync(User);

            if (identityUser != null && _context.Favorites != null)
            {
                var regularUser = await _context.RegularUsers
                    .FirstOrDefaultAsync(u => u.UserID == identityUser.Id);

                if (regularUser != null)
                {
                    UserFavoriteTeamIds = await _context.Favorites
                        .Where(f => f.NormalFK == regularUser.Id)
                        .Select(f => f.TeamFK)
                        .ToListAsync();
                }
            }
        }

        /// <summary>
        /// Alterna o estado de favorito de uma equipa para o utilizador autenticado, sem causar perdas de exceções na BD.
        /// </summary>
        public async Task<IActionResult> OnPostToggleFavoriteAsync(int teamId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                return Challenge();
            }

            var teamExists = await _context.Teams.AnyAsync(t => t.Id == teamId);
            if (!teamExists)
            {
                return NotFound();
            }

            var regularUser = await _context.RegularUsers
                .FirstOrDefaultAsync(u => u.UserID == identityUser.Id);

            if (regularUser == null)
            {
                return BadRequest("Perfil de utilizador não configurado ou o utilizador atual é um Administrador.");
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.NormalFK == regularUser.Id && f.TeamFK == teamId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
            }
            else
            {
                var newFavorite = new Favorite
                {
                    NormalFK = regularUser.Id,
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
                // Tratamento seguro de exceções de acordo com o padrão do professor (sem stack traces cruas)
                return StatusCode(500, "Ocorreu um erro interno ao processar a operação de favoritos. Contacte administrador.");
            }

            // Redireciona de volta preservando o estado de paginação atual do Utilizador
            return RedirectToPage("./Index", new { paginaAtual = PaginaAtual });
        }
    }
}