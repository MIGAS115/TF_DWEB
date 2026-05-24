using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Data.Seed
{
    /// <summary>
    /// Classe responsável pela população inicial (Seeding) da base de dados do projeto.
    /// </summary>
    internal class DbInitializer
    {
        /// <summary>
        /// Alimenta as tabelas do Identity e de domínio caso estejam vazias, utilizando o padrão haAdicao para otimização de I/O.
        /// </summary>
        internal static async Task InitializeAsync(ApplicationDbContext dbContext, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.EnsureCreated();

            bool haAdicao = false;

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Normal"))
            {
                await roleManager.CreateAsync(new IdentityRole("Normal"));
            }

            var defaultAdminEmail = "admin@esports.pt";
            var adminUser = await userManager.FindByEmailAsync(defaultAdminEmail);

            if (adminUser == null)
            {
                var appAdmin = new Admin
                {
                    UserName = defaultAdminEmail,
                    Email = defaultAdminEmail,
                    FullName = "Administrador Master",
                    EmailConfirmed = true,
                    PermissionLevel = "SuperAdmin"
                };

                var result = await userManager.CreateAsync(appAdmin, "Admin_123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(appAdmin, "Admin");
                    haAdicao = true;
                }
            }

            if (!dbContext.Tournaments.Any())
            {
                var tournaments = new[] {
                    new Tournament { Name = "IEM Katowice 2026", GameName = "CS2", IsManualOverride = true },
                    new Tournament { Name = "Worlds 2026", GameName = "LOL", IsManualOverride = true },
                    new Tournament { Name = "The International 2026", GameName = "DOTA2", IsManualOverride = true }
                };
                await dbContext.Tournaments.AddRangeAsync(tournaments);
                haAdicao = true;
            }

            if (haAdicao)
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}