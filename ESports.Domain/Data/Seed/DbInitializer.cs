using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Data.Seed
{
    /// <summary>
    /// Classe estática responsável por gerir a população (Seed) e inicialização dos dados predefinidos do sistema.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Executa as migrações da base de dados e insere as regras, o administrador e as categorias/equipas se não existirem.
        /// </summary>
        /// <param name="dbContext">Contexto da base de dados da aplicação.</param>
        /// <param name="userManager">Gestor de utilizadores do ASP.NET Core Identity.</param>
        /// <param name="roleManager">Gestor de perfis/roles do ASP.NET Core Identity.</param>
        /// <returns>Uma Task representando a operação assíncrona.</returns>
        public static async Task Initialize(
            ApplicationDbContext dbContext,
            UserManager<MyUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));
            ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));

            // Executa migrações pendentes de forma totalmente assíncrona
            if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
            {
                await dbContext.Database.MigrateAsync();
            }

            // Variável de controlo para otimizar os SaveChanges ao fim do Seed
            bool haAdicao = false;

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("RegularUser"))
            {
                await roleManager.CreateAsync(new IdentityRole("RegularUser"));
            }

            var defaultAdminEmail = "admin@esports.pt";
            var adminUser = await userManager.FindByEmailAsync(defaultAdminEmail);

            if (adminUser == null)
            {
                var identityAdmin = new Admin
                {
                    UserName = defaultAdminEmail,
                    Email = defaultAdminEmail,
                    FullName = "Administrador Master",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(identityAdmin, "Admin_123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(identityAdmin, "Admin");
                    haAdicao = true;
                }
            }

            if (!await dbContext.Tournaments.AnyAsync())
            {
                var tournaments = new[]
                {
                    new Tournament { Name = "IEM Katowice 2026", GameName = "CS2", IsManualOverride = true },
                    new Tournament { Name = "Worlds 2026", GameName = "LOL", IsManualOverride = true },
                    new Tournament { Name = "The International 2026", GameName = "DOTA2", IsManualOverride = true }
                };
                await dbContext.Tournaments.AddRangeAsync(tournaments);
                haAdicao = true;
            }

            if (!await dbContext.Teams.AnyAsync())
            {
                var teams = new[]
                {
                    new Team { Name = "Natus Vincere", LogoPath = "navi.png", IsManualOverride = true },
                    new Team { Name = "T1 Esports", LogoPath = "t1.png", IsManualOverride = true },
                    new Team { Name = "Team Liquid", LogoPath = "liquid.png", IsManualOverride = true }
                };
                await dbContext.Teams.AddRangeAsync(teams);
                haAdicao = true;
            }

            if (haAdicao)
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}