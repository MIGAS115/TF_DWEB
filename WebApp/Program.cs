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

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/// <summary>
/// Configura o serviço Identity Framework para gestão integrada de autenticação e autorização.
/// Inclui as regras exatas de password solicitadas no guia do professor.
/// </summary>
builder.Services.AddDefaultIdentity<MyUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
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
/// Regista os serviços subjacentes ao modelo MVC com foco no pipeline das Razor Pages (App Web)
/// e Controllers (obrigatório para a especificação da Componente 2 de API).
/// </summary>
builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

/// <summary>
/// Configura os middlewares do pipeline de processamento de pedidos HTTP.
/// Em ambiente de desenvolvimento, aciona a execução do Seeding (população inicial) da base de dados.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();

    app.UseDbInitializer();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();