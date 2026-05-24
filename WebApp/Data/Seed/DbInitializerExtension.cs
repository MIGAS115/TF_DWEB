using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Data.Seed
{
    /// <summary>
    /// Classe de extensão para expor a inicialização do Seed no pipeline do Program.cs de forma síncrona.
    /// </summary>
    internal static class DbInitializerExtension
    {
        /// <summary>
        /// Cria o escopo de injeção de dependências e invoca a população da base de dados.
        /// </summary>
        public static IApplicationBuilder UseItToSeedSqlServer(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            // Criação do escopo isolado para resolver os serviços do Identity e BD
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<MyUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Executa o método assíncrono bloqueando a thread principal do pipeline
                DbInitializer.InitializeAsync(context, userManager, roleManager).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Erro fatal ao inicializar e alimentar a base de dados SQL Server.");
            }

            return app;
        }
    }
}