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

/// <summary>
/// Configuração do Identity para a gestão de utilizadores e autenticação.
/// </summary>
builder.Services.AddIdentity<MyUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

/// <summary>
/// Configuração da gestão de Sessão e Cookies.
/// (Nota: IdleTimeout definido para 1000 segundos conforme guia do professor).
/// </summary>
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromSeconds(1000); // CORRIGIDO AQUI!
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

// Na segunda secção, adicionar para começar a usar os 'cookies'
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

/// <summary>
/// Mapeamento das rotas para os respetivos controladores da API.
/// </summary>
app.MapControllers();

app.Run();