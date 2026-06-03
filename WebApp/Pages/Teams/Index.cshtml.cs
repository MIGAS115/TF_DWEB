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
        /// Coleção de equipas a exibir na listagem. Pluralizada para evitar conflitos de tipo e escopo no motor Razor.
        /// </summary>
        public IList<Team> Teams { get; set; } = [];

        /// <summary>
        /// Lista de IDs das equipas favoritas do utilizador autenticado.
        /// Utilizada na vista para ditar a renderização condicional do ícone/botão de favorito.
        /// </summary>
        public List<int> UserFavoriteTeamIds { get; set; } = [];

        /// <summary>
        /// Executa o carregamento assíncrono das equipas registadas e mapeia os favoritos do utilizador logado.
        /// </summary>
        public async Task OnGetAsync()
        {
            if (_context.Teams != null)
            {
                // Carrega a listagem completa para a propriedade pluralizada Teams
                Teams = await _context.Teams.ToListAsync();
            }

            // Obtém o utilizador atualmente autenticado (que é do tipo MyUser)
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

            // Verifica se o utilizador atual existe na tabela de utilizadores regulares
            var isRegularUser = await _context.RegularUsers.AnyAsync(n => n.Id == user.Id);

            if (!isRegularUser)
            {
                // Resposta de validação controlada que impede operações de favoritos para perfis administrativos
                return BadRequest("Perfil de utilizador não configurado ou o utilizador atual é um Administrador.");
            }

            // Procura o favorito usando diretamente o ID do utilizador autenticado
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
                // Tratamento de segurança que encapsula falhas internas sem expor dados sensíveis da infraestrutura
                return StatusCode(500, "Ocorreu um erro interno ao processar a operação de favoritos.");
            }

            return RedirectToPage("./Index");
        }
    }
}