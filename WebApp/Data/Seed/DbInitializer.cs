using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Data.Seed
{

    internal class DbInitializer
    {

        internal static async Task InitializeAsync(ApplicationDbContext dbContext, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.EnsureCreated();

            bool haAdicao = false;

            // 1. Criar Roles (Papéis)
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Normal"))
            {
                await roleManager.CreateAsync(new IdentityRole("Normal"));
            }

            // 2. Criar Utilizador Administrador no Identity via Modelo Admin
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

                await userManager.CreateAsync(appAdmin, "Admin_123!");
                await userManager.AddToRoleAsync(appAdmin, "Admin");
                haAdicao = true;
            }

            // 3. Criar Torneios Iniciais (Categorias)
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
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}