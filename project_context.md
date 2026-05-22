# Contexto do Projeto E-Sports (ASP.NET Core)

> Ficheiro auto-gerado para partilha de contexto de código.

## Ficheiro: `ESports.Domain\Data\ApplicationDbContext.cs`

```csharp
﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Models;

namespace ESports.Domain.Data;

/// <summary>
/// Contexto principal da base de dados responsável pelo mapeamento das entidades e gestão do sistema de autenticação Identity.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<MyUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Tabela representativa das equipas.
    /// </summary>
    public DbSet<Team> Teams { get; set; } = null!;

    /// <summary>
    /// Tabela representativa dos jogos agendados ou concluídos.
    /// </summary>
    public DbSet<ESports.Domain.Models.Match> Matches { get; set; } = null!;

    /// <summary>
    /// Tabela intermédia de associação N:M para registo das equipas favoritas de cada utilizador.
    /// </summary>
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Admin> Admins { get; set; } = null!;
    public DbSet<Normal> Normals { get; set; } = null!;
    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<TournamentTeam> TournamentTeams { get; set; } = null!;

    /// <summary>
    /// Configuração das regras e restrições de relacionamento da base de dados através da Fluent API.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ESports.Domain.Models.Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeamFK)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ESports.Domain.Models.Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeamFK)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## Ficheiro: `ESports.Domain\Models\Admin.cs`

```csharp
﻿using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Utilizador com privilégios de Administração (Herda de MyUser)
    /// </summary>
    public class Admin : MyUser
    {
        [Required]
        [StringLength(50)]
        public string PermissionLevel { get; set; } = "FullAccess";
    }
}
```

## Ficheiro: `ESports.Domain\Models\Favorite.cs`

```csharp
﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models
{

    /// <summary>
    /// Relacionamento M:N entre Utilizador Normal e Equipa
    /// </summary>
    [PrimaryKey(nameof(NormalUserFK), nameof(TeamFK))]
    public class Favorite
    {

        [ForeignKey(nameof(Normal))]
        public string NormalUserFK { get; set; } = string.Empty;
        public Normal Normal { get; set; } = null!;

        [ForeignKey(nameof(Team))]
        public int TeamFK { get; set; }
        public Team Team { get; set; } = null!;
    }
}
```

## Ficheiro: `ESports.Domain\Models\Match.cs`

```csharp
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models;

/// <summary>
/// Entidade que representa um jogo ou partida entre duas equipas.
/// </summary>
public class Match
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Data e Hora do Jogo")]
    [Required]
    public DateTime MatchDate { get; set; }

    [Display(Name = "Resultado Equipa Casa")]
    public int? HomeScore { get; set; }

    [Display(Name = "Resultado Equipa Fora")]
    public int? AwayScore { get; set; }

    /// <summary>
    /// Indica se o registo foi criado ou editado manualmente por um administrador.
    /// </summary>
    public bool IsManualOverride { get; set; }

    /// <summary>
    /// Identificador do registo proveniente da API externa de dados.
    /// </summary>
    public string? ExternalSourceId { get; set; }

    /// <summary>
    /// Chave estrangeira para a equipa visitada (casa).
    /// </summary>
    [ForeignKey(nameof(HomeTeam))]
    [Display(Name = "Equipa Visitada")]
    public int HomeTeamFK { get; set; }

    [InverseProperty(nameof(Team.HomeMatches))]
    public Team HomeTeam { get; set; } = null!;

    /// <summary>
    /// Chave estrangeira para a equipa visitante (fora).
    /// </summary>
    [ForeignKey(nameof(AwayTeam))]
    [Display(Name = "Equipa Visitante")]
    public int AwayTeamFK { get; set; }

    [InverseProperty(nameof(Team.AwayMatches))]
    public Team AwayTeam { get; set; } = null!;

    /// <summary>
    /// Chave estrangeira para o Torneio onde o jogo decorre.
    /// </summary>
    [ForeignKey(nameof(Tournament))]
    public int TournamentFK { get; set; }
    public Tournament Tournament { get; set; } = null!;
}
```

## Ficheiro: `ESports.Domain\Models\MyUser.cs`

```csharp
﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
namespace ESports.Domain.Models;

/// <summary>
/// Extensão do IdentityUser para incluir campos personalizados de perfil.
/// </summary>
public class MyUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
```

## Ficheiro: `ESports.Domain\Models\RegularUser.cs`

```csharp
﻿using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models
{

    /// <summary>
    /// Utilizador Normal da plataforma (Herda de MyUser)
    /// </summary>
    public class Normal : MyUser
    {

        /// <summary>
        /// Data em que o utilizador se registou
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        /* ****************************************
         * Construção dos Relacionamentos
         * *************************************** */
        public ICollection<Favorite> FavoritesList { get; set; } = [];
    }
}
```

## Ficheiro: `ESports.Domain\Models\Team.cs`

```csharp
﻿using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ESports.Domain.Models;

/// <summary>
/// Entidade que representa uma equipa de E-sports.
/// </summary>
public class Team
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O {0} é obrigatório")]
    [StringLength(50, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
    [Display(Name = "Nome da Equipa")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Nome do ficheiro correspondente ao logótipo da equipa (guardado no servidor).
    /// </summary>
    [Display(Name = "Logótipo")]
    [StringLength(255)]
    public string? LogoPath { get; set; }

    /// <summary>
    /// Indica se o registo foi inserido/editado manualmente (Redundância/Defesa).
    /// </summary>
    public bool IsManualOverride { get; set; }

    /// <summary>
    /// ID proveniente da API externa.
    /// </summary>
    public string? ExternalSourceId { get; set; }

    public ICollection<Match> HomeMatches { get; set; } = [];
    public ICollection<Match> AwayMatches { get; set; } = [];
    public ICollection<Favorite> FavoritesList { get; set; } = [];
}
```

## Ficheiro: `ESports.Domain\Models\Tournament.cs`

```csharp
﻿using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Representa um Torneio
    /// </summary>
    public class Tournament
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório!")]
        [StringLength(100)]
        [Display(Name = "Torneio")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório!")]
        [StringLength(50)]
        [Display(Name = "Jogo")]
        public string GameName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ExternalSourceId { get; set; }

        public bool IsManualOverride { get; set; }

        /* Relacionamentos */
        public ICollection<Match> MatchesList { get; set; } = [];
        // NOTA: Como ainda não têm o modelo TournamentTeam, podem deixar a linha abaixo comentada por agora, ou criar a classe no passo 2
        // public ICollection<TournamentTeam> TeamsList { get; set; } = [];
    }
}
```

## Ficheiro: `ESports.Domain\Models\TournamentTeam.cs`

```csharp
﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports.Domain.Models
{
    /// <summary>
    /// Relacionamento M:N entre Torneio e Equipa
    /// </summary>
    [PrimaryKey(nameof(TournamentFK), nameof(TeamFK))]
    public class TournamentTeam
    {
        [ForeignKey(nameof(Tournament))]
        public int TournamentFK { get; set; }
        public Tournament Tournament { get; set; } = null!;

        [ForeignKey(nameof(Team))]
        public int TeamFK { get; set; }
        public Team Team { get; set; } = null!;
    }
}
```

## Ficheiro: `WebApi\appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ESports_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## Ficheiro: `WebApi\Program.cs`

```csharp
using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configuração do contexto de base de dados partilhado utilizando SQL Server.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

/// <summary>
/// Registo dos serviços de suporte para a arquitetura baseada em Controladores (API MVC).
/// </summary>
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORREÇÃO ERRO 1: Usar 'AddIdentity' puro em vez de 'AddDefaultIdentity' na API
builder.Services.AddIdentity<MyUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// configurar o de uso de 'cookies'
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromSeconds(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// na segunda secção, adicionar para
// começar a usar, realmente, os 'cookies'
app.UseSession();


app.UseAuthentication();
app.UseAuthorization();

/// <summary>
/// Mapeamento das rotas para os respetivos controladores da API.
/// </summary>
app.MapControllers();

app.Run();
```

## Ficheiro: `WebApp\appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ESports_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## Ficheiro: `WebApp\package.json`

```json
{
  "name": "webapp",
  "version": "1.0.0",
  "description": "",
  "main": "index.js",
  "scripts": {
    "test": "echo \"Error: no test specified\" && exit 1"
  },
  "keywords": [],
  "author": "",
  "license": "ISC",
  "type": "commonjs",
  "devDependencies": {
    "eslint": "^10.3.0",
    "tailwindcss": "^3.4.19"
  }
}

```

## Ficheiro: `WebApp\Program.cs`

```csharp
using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configuração do contexto de base de dados utilizando SQL Server.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/// <summary>
/// Configuração do sistema de autenticação e gestão de utilizadores com modelo personalizado.
/// </summary>
builder.Services.AddDefaultIdentity<MyUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

/// <summary>
/// Registo dos serviços necessários para o funcionamento das Razor Pages.
/// </summary>
builder.Services.AddRazorPages();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    await app.UseItToSeedSqlServerAsync();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
```

## Ficheiro: `WebApp\Areas\Identity\Pages\_ViewStart.cshtml`

```html
﻿@{
    Layout = "/Pages/Shared/_Layout.cshtml";
}

```

## Ficheiro: `WebApp\Data\Seed\DbInitializer.cs`

```csharp
﻿using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Data.Seed
{

    internal class DbInitializer
    {

        internal static async Task InitializeAsync(ApplicationDbContext dbContext, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.EnsureCreated();

            bool haAdicao = false;

            // 1. Criar Roles (Papéis)
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Normal"))
            {
                await roleManager.CreateAsync(new IdentityRole("Normal"));
            }

            // 2. Criar Utilizador Administrador no Identity via Modelo Admin
            var defaultAdminEmail = "admin@esports.pt";
            var adminUser = await userManager.FindByEmailAsync(defaultAdminEmail);

            if (adminUser == null)
            {
                var appAdmin = new Admin
                {
                    UserName = defaultAdminEmail,
                    Email = defaultAdminEmail,
                    FullName = "Administrador Master",
                    EmailConfirmed = true,
                    PermissionLevel = "SuperAdmin"
                };

                await userManager.CreateAsync(appAdmin, "Admin_123!");
                await userManager.AddToRoleAsync(appAdmin, "Admin");
                haAdicao = true;
            }

            // 3. Criar Torneios Iniciais (Categorias)
            if (!dbContext.Tournaments.Any())
            {
                var tournaments = new[] {
               new Tournament { Name = "IEM Katowice 2026", GameName = "CS2", IsManualOverride = true },
               new Tournament { Name = "Worlds 2026", GameName = "LOL", IsManualOverride = true },
               new Tournament { Name = "The International 2026", GameName = "DOTA2", IsManualOverride = true }
            };
                await dbContext.Tournaments.AddRangeAsync(tournaments);
                haAdicao = true;
            }

            if (haAdicao)
            {
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
```

## Ficheiro: `WebApp\Data\Seed\DbInitializerExtension.cs`

```csharp
﻿using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Data.Seed
{

    internal static class DbInitializerExtension
    {

        public static async Task UseItToSeedSqlServerAsync(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<MyUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await DbInitializer.InitializeAsync(context, userManager, roleManager);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
```

## Ficheiro: `WebApp\Pages\Error.cshtml`

```html
﻿@page
@model ErrorModel
@{
    ViewData["Title"] = "Error";
}

<h1 class="text-danger">Error.</h1>
<h2 class="text-danger">An error occurred while processing your request.</h2>

@if (Model.ShowRequestId)
{
    <p>
        <strong>Request ID:</strong> <code>@Model.RequestId</code>
    </p>
}

<h3>Development Mode</h3>
<p>
    Swapping to the <strong>Development</strong> environment displays detailed information about the error that occurred.
</p>
<p>
    <strong>The Development environment shouldn't be enabled for deployed applications.</strong>
    It can result in displaying sensitive information from exceptions to end users.
    For local debugging, enable the <strong>Development</strong> environment by setting the <strong>ASPNETCORE_ENVIRONMENT</strong> environment variable to <strong>Development</strong>
    and restarting the app.
</p>

```

## Ficheiro: `WebApp\Pages\Error.cshtml.cs`

```csharp
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}


```

## Ficheiro: `WebApp\Pages\Index.cshtml`

```html
﻿@page
@model IndexModel
@{
    ViewData["Title"] = "Home page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>

```

## Ficheiro: `WebApp\Pages\Index.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    public void OnGet()
    {

    }
}

```

## Ficheiro: `WebApp\Pages\Privacy.cshtml`

```html
﻿@page
@model PrivacyModel
@{
    ViewData["Title"] = "Privacy Policy";
}
<h1>@ViewData["Title"]</h1>

<p>Use this page to detail your site's privacy policy.</p>

```

## Ficheiro: `WebApp\Pages\Privacy.cshtml.cs`

```csharp
﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class PrivacyModel : PageModel
{
    public void OnGet()
    {
    }
}


```

## Ficheiro: `WebApp\Pages\_ViewImports.cshtml`

```html
﻿@using WebApp
@using ESports.Domain.Models
@using ESports.Domain.Data
@namespace WebApp.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

## Ficheiro: `WebApp\Pages\_ViewStart.cshtml`

```html
﻿@{
    Layout = "_Layout";
}

```

## Ficheiro: `WebApp\Pages\Shared\_Layout.cshtml`

```html
﻿<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - WebApp</title>
    <script type="importmap"></script>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/WebApp.styles.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
            <div class="container">
                <a class="navbar-brand" asp-area="" asp-page="/Index">WebApp</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Index">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-page="/Privacy">Privacy</a>
                        </li>
                    </ul>
                    <partial name="_LoginPartial" />
                </div>
            </div>
        </nav>
    </header>
    <div class="container">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            &copy; 2026 - WebApp - <a asp-area="" asp-page="/Privacy">Privacy</a>
        </div>
    </footer>

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>

    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>

```

## Ficheiro: `WebApp\Pages\Shared\_LoginPartial.cshtml`

```html
﻿@using Microsoft.AspNetCore.Identity
@using ESports.Domain.Models
@inject SignInManager<MyUser> SignInManager
@inject UserManager<MyUser> UserManager

<ul class="navbar-nav">
@if (SignInManager.IsSignedIn(User))
{
    <li class="nav-item">
        <a  class="nav-link text-dark" asp-area="Identity" asp-page="/Account/Manage/Index" title="Manage">Hello @User.Identity?.Name!</a>
    </li>
    <li class="nav-item">
        <form class="form-inline" asp-area="Identity" asp-page="/Account/Logout" asp-route-returnUrl="@Url.Page("/Index", new { area = "" })">
            <button  type="submit" class="nav-link btn btn-link text-dark">Logout</button>
        </form>
    </li>
}
else
{
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="Identity" asp-page="/Account/Register">Register</a>
    </li>
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="Identity" asp-page="/Account/Login">Login</a>
    </li>
}
</ul>

```

## Ficheiro: `WebApp\Pages\Shared\_ValidationScriptsPartial.cshtml`

```html
﻿<script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
<script src="~/lib/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js"></script>

```

## Ficheiro: `WebApp\Pages\Teams\Create.cshtml`

```html
﻿@page
@model WebApp.Pages.Teams.CreateModel

        @{
        ViewData["Title"] = "Create";
        }
        [Authorize(Roles = "Admin")]
        <h1>Create</h1>
        
    <h4>Team</h4>
    <hr />
    <div class="row">
    <div class="col-md-4">
    <form method="post">
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <span class="text-danger">*</span>
                <label asp-for="Team.Name" class="control-label"></label>
                <input asp-for="Team.Name" class="form-control" aria-required="true"/>
                <span asp-validation-for="Team.Name" class="text-danger"></span>
            </div>
            <div class="form-group form-check">
                <label class="form-check-label">
                    <input class="form-check-input" asp-for="Team.IsManualOverride" /> @Html.DisplayNameFor(model => model.Team.IsManualOverride)
                </label>
            </div>
            <div class="form-group">
                <label asp-for="Team.ExternalSourceId" class="control-label"></label>
                <input asp-for="Team.ExternalSourceId" class="form-control" />
                <span asp-validation-for="Team.ExternalSourceId" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-page="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}

```

## Ficheiro: `WebApp\Pages\Teams\Create.cshtml.cs`

```csharp
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    public class CreateModel : PageModel
    {
        private readonly ESports.Domain.Data.ApplicationDbContext _context;

        public CreateModel(ESports.Domain.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Team Team { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Teams.Add(Team);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

```

## Ficheiro: `WebApp\Pages\Teams\Delete.cshtml`

```html
﻿@page
@model WebApp.Pages.Teams.DeleteModel

@{
    ViewData["Title"] = "Delete";
}
[Authorize(Roles = "Admin")]

<h1>Delete</h1>

<h3>Are you sure you want to delete this?</h3>
<div>
    <h4>Team</h4>
    <hr />
    <dl class="row">
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Team.Name)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Team.Name)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Team.IsManualOverride)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Team.IsManualOverride)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Team.ExternalSourceId)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Team.ExternalSourceId)
        </dd>
    </dl>
    
    <form method="post">
        <input type="hidden" asp-for="Team.Id" />
        <input type="submit" value="Delete" class="btn btn-danger" /> |
        <a asp-page="./Index">Back to List</a>
    </form>
</div>

```

## Ficheiro: `WebApp\Pages\Teams\Delete.cshtml.cs`

```csharp
﻿using System;
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
```

## Ficheiro: `WebApp\Pages\Teams\Details.cshtml`

```html
﻿@page
@model WebApp.Pages.Teams.DetailsModel

@{
    ViewData["Title"] = "Details";
}

<h1>Details</h1>

<div>
    <h4>Team</h4>
    <hr />
    <dl class="row">
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Team.Name)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Team.Name)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Team.IsManualOverride)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Team.IsManualOverride)
        </dd>
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Team.ExternalSourceId)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Team.ExternalSourceId)
        </dd>
    </dl>
</div>
<div>
    <a asp-page="./Edit" asp-route-id="@Model.Team.Id">Edit</a> |
    <a asp-page="./Index">Back to List</a>
</div>

```

## Ficheiro: `WebApp\Pages\Teams\Details.cshtml.cs`

```csharp
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    public class DetailsModel : PageModel
    {
        private readonly ESports.Domain.Data.ApplicationDbContext _context;

        public DetailsModel(ESports.Domain.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Team Team { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FirstOrDefaultAsync(m => m.Id == id);

            if (team is not null)
            {
                Team = team;

                return Page();
            }

            return NotFound();
        }
    }
}

```

## Ficheiro: `WebApp\Pages\Teams\Edit.cshtml`

```html
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    /// <summary>
    /// PageModel responsável pela criação de novas equipas, restrito a administradores.
    ///
</summary>
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Team Team { get; set; } = default!;

        /// <summary>
    /// Propriedade temporária para receber o ficheiro de imagem do formulário.
    ///
</summary>
        [BindProperty]
        public IFormFile? LogoFile { get; set; }

        /// <summary>
    /// Processa a submissão do formulário, incluindo o upload da imagem.
    ///
</summary>
        public async Task<IActionResult>
    OnPostAsync()
    {
    if (!ModelState.IsValid || _context.Teams == null || Team == null)
    {
    return Page();
    }

    // Lógica de Processamento da Imagem
    if (LogoFile != null && LogoFile.Length > 0)
    {
    // Definir a pasta de destino (wwwroot/images/logos)
    string folder = Path.Combine(_environment.WebRootPath, "images", "logos");

    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

    // Gerar um nome único para evitar conflitos (GUID + nome original)
    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(LogoFile.FileName);
    string filePath = Path.Combine(folder, fileName);

    // 3. Guardar o ficheiro no disco
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
    await LogoFile.CopyToAsync(stream);
    }

    // Guardar apenas o nome do ficheiro na propriedade da Equipa
    Team.Logo = fileName;
    }

    _context.Teams.Add(Team);
    await _context.SaveChangesAsync();

    return RedirectToPage("./Index");
    }
    }
    }

```

## Ficheiro: `WebApp\Pages\Teams\Edit.cshtml.cs`

```csharp
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    public class EditModel : PageModel
    {
        private readonly ESports.Domain.Data.ApplicationDbContext _context;

        public EditModel(ESports.Domain.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team =  await _context.Teams.FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }
            Team = team;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}

```

## Ficheiro: `WebApp\Pages\Teams\Index.cshtml`

```html
﻿@page
@model WebApp.Pages.Teams.IndexModel

@{
    ViewData["Title"] = "Index";
}

<h1>Index</h1>

<p>
    <a asp-page="Create">Create New</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Team[0].Name)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Team[0].IsManualOverride)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Team[0].ExternalSourceId)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model.Team) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Name)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.IsManualOverride)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.ExternalSourceId)
            </td>
            <td>
                <a asp-page="./Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-page="./Details" asp-route-id="@item.Id">Details</a> |
                <a asp-page="./Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
}
    </tbody>
</table>

```

## Ficheiro: `WebApp\Pages\Teams\Index.cshtml.cs`

```csharp
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

namespace WebApp.Pages.Teams
{
    public class IndexModel : PageModel
    {
        private readonly ESports.Domain.Data.ApplicationDbContext _context;

        public IndexModel(ESports.Domain.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Team> Team { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Team = await _context.Teams.ToListAsync();
        }
    }
}

```

