using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ESports.Domain.Data;

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
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(roleManager);

        dbContext.Database.EnsureCreated();
        bool haAdicao = false;

        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var defaultAdminEmail = "admin@esports.pt";
        var adminUser = await userManager.FindByEmailAsync(defaultAdminEmail);

        if (adminUser == null)
        {
            var identityAdmin = new IdentityUser
            {
                UserName = defaultAdminEmail,
                Email = defaultAdminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(identityAdmin, "PasswordAdmin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(identityAdmin, "Admin");
            }
        }

        if (!await dbContext.Categories.AnyAsync())
        {
            var cs2Category = new Category { Name = "Counter-Strike 2", Code = "CS2" };
            var lolCategory = new Category { Name = "League of Legends", Code = "LOL" };
            var dotaCategory = new Category { Name = "Dota 2", Code = "DOTA2" };

            var categories = new List<Category> { cs2Category, lolCategory, dotaCategory };
            await dbContext.Categories.AddRangeAsync(categories);

            if (!await dbContext.Tournaments.AnyAsync())
            {
                var tournaments = new[]
                {
                    new Tournament { Name = "IEM Katowice 2026", GameName = "CS2", IsManualOverride = true },
                    new Tournament { Name = "Worlds 2026", GameName = "LOL", IsManualOverride = true },
                    new Tournament { Name = "The International 2026", GameName = "DOTA2", IsManualOverride = true }
                };
                await dbContext.Tournaments.AddRangeAsync(tournaments);
            }

            if (!await dbContext.Teams.AnyAsync())
            {
                var teams = new[]
                {
                    new Team { Name = "Natus Vincere", LogoPath = "navi.png", IsManualOverride = true, Category = cs2Category },
                    new Team { Name = "T1 Esports", LogoPath = "t1.png", IsManualOverride = true, Category = lolCategory },
                    new Team { Name = "Team Liquid", LogoPath = "liquid.png", IsManualOverride = true, Category = cs2Category }
                };
                await dbContext.Teams.AddRangeAsync(teams);
            }

            haAdicao = true;
        }
        else
        {
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
                var cs2 = await dbContext.Categories.FirstOrDefaultAsync(c => c.Code == "CS2" || c.Name == "Counter-Strike 2");
                var lol = await dbContext.Categories.FirstOrDefaultAsync(c => c.Code == "LOL" || c.Name == "League of Legends");

                if (cs2 != null && lol != null)
                {
                    var teams = new[]
                    {
                        new Team { Name = "Natus Vincere", LogoPath = "navi.png", IsManualOverride = true, CategoryFK = cs2.Id },
                        new Team { Name = "T1 Esports", LogoPath = "t1.png", IsManualOverride = true, CategoryFK = lol.Id },
                        new Team { Name = "Team Liquid", LogoPath = "liquid.png", IsManualOverride = true, CategoryFK = cs2.Id }
                    };
                    await dbContext.Teams.AddRangeAsync(teams);
                    haAdicao = true;
                }
            }
        }

        if (haAdicao)
        {
            await dbContext.SaveChangesAsync();
        }
    }
}