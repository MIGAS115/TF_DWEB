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

            // Forçar a criação da BD sem depender de Migrations
            dbContext.Database.EnsureCreated();

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

            // 1. INSERIR CATEGORIAS PRIMEIRO (Obrigatório para podermos associar as equipas depois)
            if (!await dbContext.Categories.AnyAsync())
            {
                var categories = new[]
                {
                    new Category { Name = "CS2" },
                    new Category { Name = "LOL" },
                    new Category { Name = "DOTA2" }
                };
                await dbContext.Categories.AddRangeAsync(categories);
                await dbContext.SaveChangesAsync(); // Guardar já para a BD gerar os IDs
                haAdicao = false; // Reset à flag porque já fizemos SaveChanges
            }

            // Ir buscar as categorias recém-criadas para associar às equipas
            var cs2Category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "CS2");
            var lolCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "LOL");

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

            if (!await dbContext.Teams.AnyAsync() && cs2Category != null && lolCategory != null)
            {
                var teams = new[]
                {
                    new Team { Name = "Natus Vincere", LogoPath = "navi.png", IsManualOverride = true, CategoryFK = cs2Category.Id },
                    new Team { Name = "T1 Esports", LogoPath = "t1.png", IsManualOverride = true, CategoryFK = lolCategory.Id },
                    new Team { Name = "Team Liquid", LogoPath = "liquid.png", IsManualOverride = true, CategoryFK = cs2Category.Id }
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