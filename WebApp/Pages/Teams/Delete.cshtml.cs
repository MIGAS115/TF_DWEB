using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    /// <summary>
    /// PageModel responsável pela remoção de equipas, garantindo a integridade referencial dos dados e eliminando os respetivos ficheiros de logótipo físicos.
    /// Restrito a Admin e Gestores, validando a posse do recurso.
    /// </summary>
    [Authorize(Roles = "Admin,Gestor")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Inicializa o PageModel injetando os contextos da base de dados e do ambiente web.
        /// </summary>
        public DeleteModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// Entidade que representa a equipa a ser avaliada para eliminação.
        /// </summary>
        [BindProperty]
        public Team Team { get; set; } = default!;

        /// <summary>
        /// Mensagem de erro a apresentar caso a eliminação seja impedida por dependências.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Processa o pedido HTTP GET para apresentar os detalhes, validando a permissão do utilizador.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FirstOrDefaultAsync(m => m.Id == id);

            if (team == null)
            {
                return NotFound();
            }

            // Validação de Ownership
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && team.OwnerId != loggedInUserId)
            {
                return Forbid();
            }

            Team = team;
            return Page();
        }

        /// <summary>
        /// Confirma a eliminação, bloqueia por dependências relacionais, elimina o ficheiro associado e atualiza a BD.
        /// </summary>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            // Validação de Ownership (Segurança contra POST falsificado)
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && team.OwnerId != loggedInUserId)
            {
                return Forbid();
            }

            Team = team;

            // Validação de Integridade Referencial
            bool hasMatches = await _context.Matches.AnyAsync(m => m.HomeTeamFK == id || m.AwayTeamFK == id);

            if (hasMatches)
            {
                ErrorMessage = "Não é possível eliminar esta equipa porque já existem jogos associados (como visitada ou visitante). Elimine os jogos primeiro.";
                ModelState.AddModelError(string.Empty, ErrorMessage);
                return Page();
            }

            try
            {
                // Eliminação Física do Logótipo (Ignorando a imagem padrão)
                if (!string.IsNullOrEmpty(Team.LogoPath) && Team.LogoPath != "default_team.png")
                {
                    string filePath = Path.Combine(_environment.WebRootPath, "images", "teams", Team.LogoPath);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Teams.Remove(Team);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao tentar eliminar a equipa. Contacte o administrador.");
            }
        }
    }
}