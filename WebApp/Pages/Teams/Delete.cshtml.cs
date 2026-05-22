using System;
using System.IO;
using System.Threading.Tasks;
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
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Inicializa o PageModel injetando os contextos da base de dados e do ambiente web.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="environment">Ambiente de alojamento web.</param>
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
        /// Mensagem de erro a apresentar caso a eliminação seja impedida por dependências de integridade relacional.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Processa o pedido HTTP GET para apresentar os detalhes da equipa a eliminar.
        /// </summary>
        /// <param name="id">Chave primária da equipa.</param>
        /// <returns>A página renderizada ou resultado NotFound se o id não existir.</returns>
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

            Team = team;

            return Page();
        }

        /// <summary>
        /// Processa o pedido HTTP POST para confirmar a eliminação.
        /// Bloqueia a ação caso existam jogos associados na tabela Matches.
        /// </summary>
        /// <param name="id">Chave primária da equipa.</param>
        /// <returns>Redireciona para o Index em caso de sucesso, ou recarrega a página com erro.</returns>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            bool hasMatches = await _context.Matches.AnyAsync(m => m.HomeTeamFK == id || m.AwayTeamFK == id);

            if (hasMatches)
            {
                Team = await _context.Teams.FirstAsync(m => m.Id == id);
                ErrorMessage = "Não é possível eliminar esta equipa porque já existem jogos associados (como visitada ou visitante). Elimine os jogos primeiro.";
                return Page();
            }

            var team = await _context.Teams.FindAsync(id);

            if (team != null)
            {
                Team = team;

                if (!string.IsNullOrEmpty(Team.LogoPath))
                {
                    string filePath = Path.Combine(_environment.WebRootPath, "images", "logos", Team.LogoPath);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Teams.Remove(Team);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}