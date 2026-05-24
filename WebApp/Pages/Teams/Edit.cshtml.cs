using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    /// <summary>
    /// PageModel responsável pela lógica de modificação e atualização de equipas existentes.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        /// <summary>
        /// Contexto de acesso à base de dados do projeto.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Ambiente de alojamento web para obtenção dos caminhos físicos do servidor.
        /// </summary>
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Construtor do PageModel com injeção de dependências.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="environment">Ambiente de alojamento web.</param>
        public EditModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// Propriedade que armazena os dados da equipa a ser editada, vinculada ao formulário.
        /// </summary>
        [BindProperty]
        public Team Team { get; set; } = null!;

        /// <summary>
        /// Propriedade temporária opcional que recebe o novo ficheiro de logótipo para substituição.
        /// </summary>
        [BindProperty]
        public IFormFile? LogoFile { get; set; }

        /// <summary>
        /// Carrega os dados da equipa com base no identificador fornecido.
        /// </summary>
        /// <param name="id">Chave primária da equipa.</param>
        /// <returns>A página preenchida ou NotFound caso o ID seja inválido.</returns>
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
        /// Processa as alterações submetidas, substitui o ficheiro físico do logótipo antigo se necessário e atualiza o registo.
        /// </summary>
        /// <returns>Redirecionamento para o Index em caso de sucesso.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (LogoFile != null && LogoFile.Length > 0)
            {
                string folder = Path.Combine(_environment.WebRootPath, "images", "logos");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var fileExtension = Path.GetExtension(LogoFile.FileName).ToLowerInvariant();
                string fileName = Guid.NewGuid().ToString() + fileExtension;
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(Team.LogoPath))
                {
                    string oldFilePath = Path.Combine(folder, Team.LogoPath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                Team.LogoPath = fileName;
            }

            Team.IsManualOverride = true;
            _context.Attach(Team).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(Team.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "A equipa foi modificada por outro utilizador. Por favor, recarregue a página.");
                    return Page();
                }
            }

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Avalia a existência de uma equipa com base no identificador.
        /// </summary>
        /// <param name="id">Chave primária da equipa.</param>
        /// <returns>Verdadeiro se a equipa existir; falso caso contrário.</returns>
        private bool TeamExists(int id)
        {
            return (_context.Teams?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}