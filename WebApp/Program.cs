using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Data;
using ESports.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configuração do contexto de base de dados utilizando SQL Server.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();