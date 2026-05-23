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
/// </summary>
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromSeconds(1000); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configura o Swagger para incluir os comentários XML da tua API
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseHttpsRedirection();

// Na segunda secção, adicionar para começar a usar os 'cookies'
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

/// <summary>
/// Mapeamento das rotas para os respetivos controladores da API.
/// </summary>
app.MapControllers();

app.Run();