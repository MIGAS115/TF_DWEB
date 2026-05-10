using ESports.Domain.Data;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

/// <summary>
/// Mapeamento das rotas para os respetivos controladores da API.
/// </summary>
app.MapControllers();

app.Run();