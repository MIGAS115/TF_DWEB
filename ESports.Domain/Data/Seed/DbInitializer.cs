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
        public static async Task Initialize(
            ApplicationDbContext dbContext,
            UserManager<MyUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));
            ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));

            dbContext.Database.EnsureCreated();
            bool haAdicao = false;

            // Roles
            string[] roleNames = { "Admin", "Gestor", "RegularUser" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Utilizador Admin — MyUser como entidade Identity direta (tua arquitetura)
            var defaultAdminEmail = "admin@esports.pt";
            if (await userManager.FindByEmailAsync(defaultAdminEmail) == null)
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

            // Estruturas em memória (padrão do colega — mais limpo, sem SaveChanges a meio)
            Category? cs2Category = null;
            Category? lolCategory = null;

            if (!await dbContext.Categories.AnyAsync())
            {
                cs2Category = new Category { Name = "CS2" };
                lolCategory = new Category { Name = "LOL" };
                var dotaCategory = new Category { Name = "DOTA2" };

                await dbContext.Categories.AddRangeAsync(cs2Category, lolCategory, dotaCategory);
                haAdicao = true;
            }
            else
            {
                cs2Category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "CS2");
                lolCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "LOL");
            }

            // Torneios
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

            // Equipas — propriedade de navegação
            if (!await dbContext.Teams.AnyAsync() && cs2Category != null && lolCategory != null)
            {
                var teams = new[]
                {
                    new Team { Name = "Natus Vincere", LogoPath = "navi.png", IsManualOverride = true, Category = cs2Category },
                    new Team { Name = "T1 Esports", LogoPath = "t1.png", IsManualOverride = true, Category = lolCategory },
                    new Team { Name = "Team Liquid", LogoPath = "liquid.png", IsManualOverride = true, Category = cs2Category }
                };
                await dbContext.Teams.AddRangeAsync(teams);
                haAdicao = true;
            }
            if (haAdicao)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        internal static object Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            throw new NotImplementedException();
        }
    }
}