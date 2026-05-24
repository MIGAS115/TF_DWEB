using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configuração e inicialização da cadeia de ligação à base de dados relacional SQL Server.
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

/// <summary>
/// Configuração e registo do filtro de diagnóstico de erros do Entity Framework Core para ambiente de desenvolvimento.
/// </summary>
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/// <summary>
/// Configuração do ecossistema ASP.NET Core Identity para a gestão de utilizadores, autenticação e controlo de perfis de acesso.
/// </summary>
builder.Services.AddIdentity<MyUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

/// <summary>
/// Configuração da infraestrutura de cache em memória interna distribuída, necessária para o suporte do estado de sessão.
/// </summary>
builder.Services.AddDistributedMemoryCache();

/// <summary>
/// Configuração do serviço de gestão de estados de sessão e parâmetros de segurança dos cookies associados, com validade de 1000 segundos.
/// </summary>
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

/// <summary>
/// Registo dos serviços fundamentais de suporte à arquitetura baseada em Controladores para a Componente de API.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

/// <summary>
/// Configuração e inicialização do gerador de documentação Swagger, configurado para processar os metadados e os ficheiros XML gerados.
/// </summary>
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API E-Sports IPT",
        Version = "v1",
        Description = "API para gestão de torneios, equipas e jogos de e-sports"
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

/// <summary>
/// Configuração do pipeline de execução HTTP (Middleware-Pipeline) em ambiente de desenvolvimento local.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseRouting();

/// <summary>
/// Injeção do middleware de estado de sessão, posicionado obrigatoriamente após a definição de rotas e antes dos validadores de identidade.
/// </summary>
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

/// <summary>
/// Mapeamento definitivo dos endpoints e padrões de roteamento destinados aos controladores da API.
/// </summary>
app.MapControllers();

app.Run();