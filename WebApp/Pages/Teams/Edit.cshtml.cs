using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams;

/// <summary>
/// PageModel responsável pela lógica de modificação e atualização de equipas existentes.
/// Restrito a utilizadores com os cargos Admin ou Gestor, validando o Ownership do recurso.
/// </summary>
[Authorize(Roles = "Admin,Gestor")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Construtor do PageModel com injeção de dependências.
    /// </summary>
    public EditModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    /// <summary>
    /// Propriedade que armazena os dados da equipa a ser editada.
    /// </summary>
    [BindProperty]
    public Team Team { get; set; } = null!;

    /// <summary>
    /// Propriedade temporária opcional que recebe o novo ficheiro de logótipo para substituição.
    /// </summary>
    [BindProperty]
    public IFormFile? LogoFile { get; set; }

    /// <summary>
    /// Lista de categorias disponíveis para atualizar a associação da equipa.
    /// </summary>
    public SelectList CategoryList { get; set; } = default!;

    /// <summary>
    /// Carrega os dados da equipa validando as permissões do utilizador autenticado.
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
        CategoryList = new SelectList(_context.Categories, "Id", "Name");

        return Page();
    }

    /// <summary>
    /// Processa as alterações, gere a substituição do logótipo eliminando o antigo e atualiza a BD.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            CategoryList = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }

        var teamToUpdate = await _context.Teams.FirstOrDefaultAsync(t => t.Id == Team.Id);
        if (teamToUpdate == null)
        {
            return NotFound();
        }

        // Validação de Ownership (Segurança contra POST falsificado)
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && teamToUpdate.OwnerId != loggedInUserId)
        {
            return Forbid();
        }

        teamToUpdate.Name = Team.Name;
        teamToUpdate.CategoryFK = Team.CategoryFK;
        if (LogoFile != null && LogoFile.Length > 0)
        {
            var fileExtension = Path.GetExtension(LogoFile.FileName).ToLowerInvariant();
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };

            if (!extensoesPermitidas.Contains(fileExtension))
            {
                ModelState.AddModelError(string.Empty, "O ficheiro enviado não é uma imagem válida.");
                CategoryList = new SelectList(_context.Categories, "Id", "Name");
                return Page();
            }

            string folder = Path.Combine(_environment.WebRootPath, "images", "teams");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = Guid.NewGuid().ToString().ToLowerInvariant() + fileExtension;
            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await LogoFile.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(teamToUpdate.LogoPath) && teamToUpdate.LogoPath != "default_team.png")
            {
                string oldFilePath = Path.Combine(folder, teamToUpdate.LogoPath);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            teamToUpdate.LogoPath = fileName;
        }

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeamExists(Team.Id))
            {
                return NotFound();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "A equipa foi modificada por outro utilizador.");
                CategoryList = new SelectList(_context.Categories, "Id", "Name");
                return Page();
            }
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno ao atualizar a equipa. Contacte o administrador.");
        }
    }

    private bool TeamExists(int id)
    {
        return (_context.Teams?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}