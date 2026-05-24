using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    /// <summary>
    /// PageModel responsável pela listagem de equipas e gestão do estado de favoritos dos utilizadores.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MyUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<MyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Coleção de equipas a exibir na listagem.
        /// </summary>
        public IList<Team> Team { get; set; } = [];

        /// <summary>
        /// Lista de IDs das equipas favoritas do utilizador autenticado.
        /// Utilizada na vista para ditar a renderização condicional do ícone/botão de favorito.
        /// </summary>
        public List<int> UserFavoriteTeamIds { get; set; } = [];

        public async Task OnGetAsync()
        {
            if (_context.Teams != null)
            {
                Team = await _context.Teams.ToListAsync();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user != null && _context.Favorites != null)
            {
                UserFavoriteTeamIds = await _context.Favorites
                    .Where(f => f.UserFK == user.Id)
                    .Select(f => f.TeamFK)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Alterna o estado de favorito de uma equipa para o utilizador autenticado.
        /// Protege o pipeline contra exceções de integridade relacional concorrentes.
        /// </summary>
        /// <param name="teamId">ID da equipa selecionada.</param>
        /// <returns>Recarrega a página atual com o estado atualizado.</returns>
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

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserFK == user.Id && f.TeamFK == teamId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
            }
            else
            {
                var newFavorite = new Favorite
                {
                    UserFK = user.Id,
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
                return StatusCode(500, "Erro interno ao processar a operação de favoritos.");
            }

            return RedirectToPage("./Index");
        }
    }
}