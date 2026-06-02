using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApp.Data.Seed;

/// <summary>
/// Classe de extensão para expor a inicialização do Seed no pipeline do Program.cs de forma síncrona.
/// </summary>
public static class DbInitializerExtension
{
    /// <summary>
    /// Cria o escopo de injeção de dependências e invoca a população da base de dados.
    /// </summary>
    /// <param name="app">Interface do construtor de aplicações web.</param>
    /// <returns>O IApplicationBuilder configurado para encadeamento de chamadas.</returns>
    public static IApplicationBuilder UseItToSeedSqlServer(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));

        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<MyUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            DbInitializer.InitializeAsync(context, userManager, roleManager).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<DbInitializer>>();
            logger.LogError(ex, "Erro fatal ao inicializar e alimentar a base de dados SQL Server no pipeline.");
        }

        return app;
    }
}