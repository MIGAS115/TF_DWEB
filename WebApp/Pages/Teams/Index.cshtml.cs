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
        /// PageModel responsável pela listagem de equipas e gestão do estado de favoritos dos utilizadores.
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

            public IList<Team> Teams { get; set; } = [];

            public List<int> UserFavoriteTeamIds { get; set; } = [];

            /// <summary>
            /// Lista de IDs de equipas que o utilizador autenticado tem permissão para modificar (Editar/Eliminar).
            /// </summary>
            public HashSet<int> EditableTeamIds { get; set; } = new HashSet<int>();

            /// <summary>
            /// Executa o carregamento assíncrono das equipas registadas, mapeia as permissões de gestão e os favoritos.
            /// </summary>
            public async Task OnGetAsync()
            {
                if (_context.Teams != null)
                {
                    Teams = await _context.Teams.ToListAsync();
                }

                // Lógica de Ownership para a interface
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                foreach (var team in Teams)
                {
                    if (isAdmin || (!string.IsNullOrEmpty(loggedInUserId) && team.OwnerId == loggedInUserId))
                    {
                        EditableTeamIds.Add(team.Id);
                    }
                }

                // Obtém a credencial de segurança autenticada para Favoritos
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
            /// Alterna o estado de favorito de uma equipa para o utilizador autenticado.
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
                    return StatusCode(500, "Ocorreu um erro interno ao processar a operação de favoritos.");
                }

                return RedirectToPage("./Index");
            }
        }
    }