using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Estabelece a ligação à base de dados SQL Server através do Entity Framework Core.
/// Valida a existência da connection string, requisito essencial para a persistência e integridade dos dados.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A connection string 'DefaultConnection' não foi encontrada na configuração da aplicação.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/// <summary>
/// Configura o serviço Identity Framework para gestão integrada de autenticação e autorização.
/// Inclui o registo explícito do RoleManager via AddRoles, garantindo o suporte à gestão de papéis hierárquicos e inicialização do DbInitializer.
/// </summary>
builder.Services.AddDefaultIdentity<MyUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

/// <summary>
/// Configura os serviços de estado de sessão HTTP obrigatórios da plataforma com expiração rígida.
/// </summary>
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

/// <summary>
/// Regista os serviços subjacentes ao modelo MVC com foco no pipeline das Razor Pages, 
/// em conjunto com os componentes interativos para a renderização servidora.
/// </summary>
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

/// <summary>
/// Configura os middlewares do pipeline de processamento de pedidos HTTP.
/// Em ambiente de desenvolvimento, aciona a execução do Seeding (população inicial) da base de dados.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    await app.SeedDataAsync();
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
app.UseSession();
app.MapRazorPages();
app.Run();