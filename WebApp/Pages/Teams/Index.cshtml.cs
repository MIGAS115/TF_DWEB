using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    /// <summary>
    /// PageModel responsável por listar todas as equipas disponíveis e processar as ações de adicionar ou remover favoritos (relação N:M) para utilizadores autenticados.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MyUser> _userManager;

        /// <summary>
        /// Inicializa o PageModel injetando os serviços da base de dados e de gestão de utilizadores (Identity).
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do ASP.NET Core Identity.</param>
        public IndexModel(ApplicationDbContext context, UserManager<MyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Coleção de equipas a serem apresentadas na grelha de dados.
        /// </summary>
        public IList<Team> Team { get; set; } = default!;

        /// <summary>
        /// Lista que armazena os identificadores das equipas que o utilizador atual marcou como favoritas.
        /// </summary>
        public List<int> UserFavoriteTeamIds { get; set; } = new List<int>();

        /// <summary>
        /// Processa o pedido HTTP GET para popular a lista de equipas. 
        /// Caso o utilizador esteja autenticado, preenche também a lista de favoritos.
        /// </summary>
        /// <returns>A Tarefa assíncrona da operação.</returns>
        public async Task OnGetAsync()
        {
            if (_context.Teams != null)
            {
                Team = await _context.Teams.ToListAsync();
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                if (!string.IsNullOrEmpty(userId))
                {
                    UserFavoriteTeamIds = await _context.Favorites
                        .Where(f => f.UserFK == userId)
                        .Select(f => f.TeamFK)
                        .ToListAsync();
                }
            }
        }

        /// <summary>
        /// Processa o pedido HTTP POST para alternar o estado de favorito de uma equipa específica.
        /// Adiciona o registo à tabela Favorites se não existir, ou remove caso já esteja presente.
        /// </summary>
        /// <param name="teamId">A chave primária da equipa alvo.</param>
        /// <returns>Redireciona para a própria página de Index após a alteração.</returns>
        public async Task<IActionResult> OnPostToggleFavoriteAsync(int teamId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserFK == userId && f.TeamFK == teamId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
            }
            else
            {
                var newFavorite = new Favorite
                {
                    UserFK = userId,
                    TeamFK = teamId
                };
                _context.Favorites.Add(newFavorite);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}