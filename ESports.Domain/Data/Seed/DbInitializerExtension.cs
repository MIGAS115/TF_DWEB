using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Data.Seed
{
    /// <summary>
    /// Fornece métodos de extensão para encapsular e injetar a inicialização da base de dados no pipeline da aplicação.
    /// </summary>
    public static class DbInitializerExtension
    {
        /// <summary>
        /// Cria o escopo isolado de IoC e invoca o motor de população inicial de dados de forma assíncrona.
        /// </summary>
        /// <param name="app">O construtor do pipeline de componentes da aplicação.</param>
        /// <returns>O mesmo builder da aplicação para encadeamento de métodos.</returns>
        public static async Task<IApplicationBuilder> SeedDataAsync(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<MyUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await DbInitializer.InitializeAsync(dbContext, userManager, roleManager);

            return app;
        }
    }
}