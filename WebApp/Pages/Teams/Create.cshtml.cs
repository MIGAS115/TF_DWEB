using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams;

/// <summary>
/// PageModel responsável pela lógica de criação de novas equipas no sistema.
/// </summary>
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
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
    public CreateModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    /// <summary>
    /// Propriedade que armazena os dados da equipa a ser criada, vinculada ao formulário.
    /// </summary>
    [BindProperty]
    public Team Team { get; set; } = null!;

    /// <summary>
    /// Propriedade temporária que recebe o ficheiro de imagem do logótipo enviado via formulário.
    /// </summary>
    [BindProperty]
    public IFormFile? LogoFile { get; set; }

    /// <summary>
    /// Disponibiliza e inicializa a página com o formulário de criação.
    /// </summary>
    /// <returns>O resultado da página Razor.</returns>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>
    /// Processa a submissão do formulário via HTTP POST, realiza o upload do ficheiro e persiste a entidade.
    /// </summary>
    /// <returns>Redirecionamento para o Index em caso de sucesso; caso contrário, recarrega a página com validações.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.Teams == null || Team == null)
        {
            return Page();
        }

        if (LogoFile != null && LogoFile.Length > 0)
        {
            var fileExtension = Path.GetExtension(LogoFile.FileName).ToLowerInvariant();
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };

            if (!extensoesPermitidas.Contains(fileExtension))
            {
                ModelState.AddModelError(string.Empty, "O ficheiro enviado não é uma imagem válida (.jpg, .jpeg, .png, .gif, .svg).");
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

            Team.LogoPath = fileName;
        }
        else
        {
            Team.LogoPath = "default_team.png";
        }

        Team.IsManualOverride = true;

        _context.Teams.Add(Team);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}