using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ESports.Domain.Data;
using ESports.Domain.Models;
using System.IO;

namespace WebApp.Pages.Teams
{
    /// <summary>
    /// PageModel responsável pela remoção de equipas e dos respetivos ficheiros de logo.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ESports.Domain.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DeleteModel(ESports.Domain.Data.ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Team Team { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FirstOrDefaultAsync(m => m.Id == id);

            if (team == null)
            {
                return NotFound();
            }
            else
            {
                Team = team;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);

            if (team != null)
            {
                Team = team;

                // 1. Lógica para apagar o ficheiro físico da imagem
                if (!string.IsNullOrEmpty(Team.Logo))
                {
                    string filePath = Path.Combine(_environment.WebRootPath, "images", "logos", Team.Logo);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // 2. Remover o registo da Base de Dados
                _context.Teams.Remove(Team);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}