using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams;

/// <summary>
/// PageModel responsável pela lógica de criação de novas equipas no sistema.
/// Restrito a utilizadores com cargos de gestão.
/// </summary>
[Authorize(Roles = "Admin,Gestor")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Construtor do PageModel com injeção de dependências.
    /// </summary>
    public CreateModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    /// <summary>
    /// Propriedade que armazena os dados da equipa a ser criada.
    /// </summary>
    [BindProperty]
    public Team Team { get; set; } = null!;

    /// <summary>
    /// Ficheiro de imagem do logótipo submetido.
    /// </summary>
    [BindProperty]
    public IFormFile? LogoFile { get; set; }

    /// <summary>
    /// Lista de categorias disponíveis para associar à equipa.
    /// </summary>
    public SelectList CategoryList { get; set; } = default!;

    /// <summary>
    /// Disponibiliza e inicializa a página com o formulário, carregando os dados relacionais.
    /// </summary>
    public IActionResult OnGet()
    {
        // Prepara a dropdown de categorias para o utilizador escolher (CS2, LOL, etc.)
        CategoryList = new SelectList(_context.Categories, "Id", "Name");
        return Page();
    }

    /// <summary>
    /// Processa o upload da imagem, sanitiza os dados e persiste a entidade garantindo ownership.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.Teams == null || Team == null)
        {
            CategoryList = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }

        // Processamento do Upload conforme regras do guia de estilo
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

            Team.LogoPath = fileName;
        }
        else
        {
            Team.LogoPath = "default_team.png";
        }

        // Atribuição de regras de negócio e Ownership de segurança
        Team.IsManualOverride = true;
        Team.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            _context.Teams.Add(Team);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno ao guardar equipa. Contacte o administrador.");
        }
    }
}