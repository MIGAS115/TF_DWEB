# Contexto do Projeto E-Sports (ASP.NET Core)

> Ficheiro auto-gerado para partilha de contexto de código.

## Ficheiro: `ESports.Domain\Data\ApplicationDbContext.cs`

```csharp
﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ESports.Domain.Data;

public class ApplicationDbContext : IdentityDbContext<MyUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
    /*...*/
  }

  public DbSet<Team> Teams { get; set; } = null!;

  public DbSet<ESports.Domain.Models.Match> Matches { get; set; } = null!;

  public DbSet<Favorite> Favorites { get; set; }
  public DbSet<Admin> Admins { get; set; } = null!;
  public DbSet<Normal> Normals { get; set; } = null!;
  public DbSet<Tournament> Tournaments { get; set; } = null!;
  public DbSet<TournamentTeam> TournamentTeams { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder builder)
  {
    /*...*/
  }
}
```

## Ficheiro: `ESports.Domain\Models\Admin.cs`

```csharp
﻿using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models
{
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

namespace ESports.Domain.Models
{

  [PrimaryKey(nameof(UserFK), nameof(TeamFK))]
  public class Favorite
  {

    [ForeignKey(nameof(Normal))]
    public string UserFK { get; set; } = string.Empty;
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

namespace ESports.Domain.Models;

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

  public bool IsManualOverride { get; set; }

  public string? ExternalSourceId { get; set; }

  [ForeignKey(nameof(HomeTeam))]
  [Display(Name = "Equipa Visitada")]
  public int HomeTeamFK { get; set; }

  [InverseProperty(nameof(Team.HomeMatches))]
  public Team HomeTeam { get; set; } = null!;

  [ForeignKey(nameof(AwayTeam))]
  [Display(Name = "Equipa Visitante")]
  public int AwayTeamFK { get; set; }

  [InverseProperty(nameof(Team.AwayMatches))]
  public Team AwayTeam { get; set; } = null!;

  [ForeignKey(nameof(Tournament))]
  public int TournamentFK { get; set; }
  public Tournament Tournament { get; set; } = null!;
}
```

## Ficheiro: `ESports.Domain\Models\MyUser.cs`

```csharp
﻿using Microsoft.AspNetCore.Identity;
namespace ESports.Domain.Models;

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

  public class Normal : MyUser
  {

    [DataType(DataType.Date)]
    public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public ICollection<Favorite> FavoritesList { get; set; } = [];
  }
}
```

## Ficheiro: `ESports.Domain\Models\Team.cs`

```csharp
﻿using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models;

public class Team
{
  [Key]
  public int Id { get; set; }

  [Required(ErrorMessage = "O {0} é obrigatório")]
  [StringLength(50, ErrorMessage = "O {0} não pode exceder {1} caracteres.")]
  [Display(Name = "Nome da Equipa")]
  public string Name { get; set; } = string.Empty;

  [Display(Name = "Logótipo")]
  [StringLength(255)]
  public string? LogoPath { get; set; }

  public string? ExternalSourceId { get; set; }

  public bool IsManualOverride { get; set; }

  public ICollection<Match> HomeMatches { get; set; } = new List<Match>();

  public ICollection<Match> AwayMatches { get; set; } = new List<Match>();

  public ICollection<Favorite> FavoritedBy { get; set; } = new List<Favorite>();
}
```

## Ficheiro: `ESports.Domain\Models\Tournament.cs`

```csharp
﻿using System.ComponentModel.DataAnnotations;

namespace ESports.Domain.Models
{
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

    public ICollection<Match> MatchesList { get; set; } = [];
  }
}
```

## Ficheiro: `ESports.Domain\Models\TournamentTeam.cs`

```csharp
﻿using Microsoft.EntityFrameworkCore;

namespace ESports.Domain.Models
{
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

## Ficheiro: `WebApi\Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
  ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddIdentity<MyUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

builder.Services.AddSession(options => {
  options.IdleTimeout = TimeSpan.FromSeconds(1000); 
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseHttpsRedirection();

app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

## Ficheiro: `WebApi\Controllers\CategoriesController.cs`

```csharp
﻿namespace WebApi.Controllers
{
  public class CategoriesController
  {
  }
}
```

## Ficheiro: `WebApi\Controllers\MatchesController.cs`

```csharp
﻿using ESports.Domain.Data;

namespace WebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MatchesController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public MatchesController(ApplicationDbContext context)
    {
      /*...*/
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatches()
    {
      /*...*/
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MatchDTO>> GetMatch(int id)
    {
      /*...*/
    }

    [HttpPost]
    public async Task<ActionResult<MatchDTO>> PostMatch(MatchCreateDTO matchCreateDto)
    {
      /*...*/
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutMatch(int id, MatchCreateDTO matchUpdateDto)
    {
      /*...*/
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMatch(int id)
    {
      /*...*/
    }
  }
}
```

## Ficheiro: `WebApi\Controllers\TeamsController.cs`

```csharp
﻿using ESports.Domain.Data;

namespace WebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TeamsController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public TeamsController(ApplicationDbContext context)
    {
      /*...*/
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeams()
    {
      /*...*/
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDTO>> GetTeam(int id)
    {
      /*...*/
    }

    [HttpPost]
    public async Task<ActionResult<TeamDTO>> PostTeam(TeamCreateDTO teamCreateDto)
    {
      /*...*/
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTeam(int id, TeamCreateDTO teamUpdateDto)
    {
      /*...*/
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
      /*...*/
    }
  }
}
```

## Ficheiro: `WebApi\Models\ViewModels\MatchCreateDTO.cs`

```csharp
﻿namespace WebApi.Models.ViewModels
{
  public class MatchCreateDTO
  {
    public int HomeTeamFK { get; set; }

    public int AwayTeamFK { get; set; }

    public DateTime MatchDate { get; set; }

    public int TournamentFK { get; set; }
  }
}
```

## Ficheiro: `WebApi\Models\ViewModels\MatchDTO.cs`

```csharp
﻿namespace WebApi.Models.ViewModels
{
  public class MatchDTO
  {
    public int Id { get; set; }

    public string HomeTeamName { get; set; } = string.Empty;

    public string AwayTeamName { get; set; } = string.Empty;

    public DateTime MatchDate { get; set; }
  }
}
```

## Ficheiro: `WebApi\Models\ViewModels\TeamCreateDTO.cs`

```csharp
﻿namespace WebApi.Models.ViewModels
{
  public class TeamCreateDTO
  {
    public string Name { get; set; } = string.Empty;

    public string? LogoPath { get; set; }
  }
}
```

## Ficheiro: `WebApi\Models\ViewModels\TeamDTO.cs`

```csharp
﻿namespace WebApi.Models.ViewModels
{
  public class TeamDTO
  {
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? LogoPath { get; set; }

    public int TotalFavorites { get; set; }

    public int TotalMatches { get; set; }
  }
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
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
  ?? throw new InvalidOperationException("A connection string 'DefaultConnection' não foi encontrada na configuração da aplicação.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<MyUser>(options => options.SignIn.RequireConfirmedAccount = true)
  .AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

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

## Ficheiro: `WebApp\Data\Seed\DbInitializer.cs`

```csharp
﻿using ESports.Domain.Data;

namespace WebApp.Data.Seed
{

  internal class DbInitializer
  {

    internal static async Task InitializeAsync(ApplicationDbContext dbContext, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      /*...*/
    }
  }
}
```

## Ficheiro: `WebApp\Data\Seed\DbInitializerExtension.cs`

```csharp
﻿using ESports.Domain.Data;

namespace WebApp.Data.Seed
{

  internal static class DbInitializerExtension
  {

    public static async Task UseItToSeedSqlServerAsync(this IApplicationBuilder app)
    {
      /*...*/
    }
  }
}
```

## Ficheiro: `WebApp\Pages\Error.cshtml.cs`

```csharp
namespace WebApp.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
  public string? RequestId { get; set; }

  public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

  public void OnGet()
  {
    /*...*/
  }
}
```

## Ficheiro: `WebApp\Pages\Index.cshtml.cs`

```csharp
namespace WebApp.Pages;

public class IndexModel : PageModel
{
  public void OnGet()
  {
    /*...*/
  }
}
```

## Ficheiro: `WebApp\Pages\Privacy.cshtml.cs`

```csharp
﻿using Microsoft.AspNetCore.Mvc;

namespace WebApp.Pages;

public class PrivacyModel : PageModel
{
  public void OnGet()
  {
    /*...*/
  }
}
```

## Ficheiro: `WebApp\Pages\Teams\Create.cshtml.cs`

```csharp
﻿using System;

namespace WebApp.Pages.Teams
{
  [Authorize(Roles = "Admin")]
  public class CreateModel : PageModel
  {
    private readonly ApplicationDbContext _context;

    private readonly IWebHostEnvironment _environment;

    public CreateModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
      /*...*/
    }

    [BindProperty]
    public Team Team { get; set; } = null!;

    [BindProperty]
    public IFormFile? LogoFile { get; set; }

    public IActionResult OnGet()
    {
      /*...*/
    }

    public async Task<IActionResult> OnPostAsync()
    {
      /*...*/
    }
  }
}
```

## Ficheiro: `WebApp\Pages\Teams\Delete.cshtml.cs`

```csharp
﻿using System;

namespace WebApp.Pages.Teams
{
  [Authorize(Roles = "Admin")]
  public class DeleteModel : PageModel
  {
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public DeleteModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
      /*...*/
    }

    [BindProperty]
    public Team Team { get; set; } = default!;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
      /*...*/
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
      /*...*/
    }
  }
}
```

## Ficheiro: `WebApp\Pages\Teams\Details.cshtml.cs`

```csharp
﻿using System;

namespace WebApp.Pages.Teams
{
  public class DetailsModel : PageModel
  {
    private readonly ESports.Domain.Data.ApplicationDbContext _context;

    public DetailsModel(ESports.Domain.Data.ApplicationDbContext context)
    {
      /*...*/
    }

    public Team Team { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
      /*...*/
    }
  }
}
```

## Ficheiro: `WebApp\Pages\Teams\Edit.cshtml.cs`

```csharp
﻿using System;

namespace WebApp.Pages.Teams
{
  [Authorize(Roles = "Admin")]
  public class EditModel : PageModel
  {
    private readonly ApplicationDbContext _context;

    private readonly IWebHostEnvironment _environment;

    public EditModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
      /*...*/
    }

    [BindProperty]
    public Team Team { get; set; } = null!;

    [BindProperty]
    public IFormFile? LogoFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
      /*...*/
    }

    public async Task<IActionResult> OnPostAsync()
    {
      /*...*/
    }

    private bool TeamExists(int id)
    {
      /*...*/
    }
  }
}
```

## Ficheiro: `WebApp\Pages\Teams\Index.cshtml.cs`

```csharp
﻿using System.Collections.Generic;

namespace WebApp.Pages.Teams
{
  public class IndexModel : PageModel
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<MyUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<MyUser> userManager)
    {
      /*...*/
    }

    public IList<Team> Team { get; set; } = default!;

    public List<int> UserFavoriteTeamIds { get; set; } = new List<int>();

    public async Task OnGetAsync()
    {
      /*...*/
    }

    public async Task<IActionResult> OnPostToggleFavoriteAsync(int teamId)
    {
      /*...*/
    }
  }
}
```

