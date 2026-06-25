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
        UserManager<MyUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        /// <summary>
        /// Executa as migrações da base de dados e insere as regras, o administrador e as categorias/equipas se não existirem.
        /// </summary>
        public static async Task Initialize(
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));
            ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));

            dbContext.Database.EnsureCreated();
            bool haAdicao = false;

            // Roles
            string[] roleNames = { "Admin", "Gestor", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        if (!await dbContext.Categories.AnyAsync())
        {
            var cs2Category = new Category { Name = "CS2" };
            var lolCategory = new Category { Name = "LOL" };
            var dotaCategory = new Category { Name = "DOTA2" };

            // Utilizador Admin
            var defaultAdminEmail = "admin@esports.pt";
            if (await userManager.FindByEmailAsync(defaultAdminEmail) == null)
            {
                var newIdentityAdmin = new IdentityUser
                {
                    UserName = defaultAdminEmail,
                    Email = defaultAdminEmail,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(newIdentityAdmin, "Admin_123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newIdentityAdmin, "Admin");

                    var adminProfile = new Admin
                    {
                        FullName = "Administrador Master",
                        PermissionLevel = "FullAccess",
                        UserID = newIdentityAdmin.Id
                    };

                    dbContext.Admins.Add(adminProfile);
                    haAdicao = true;
                }
            }

            // Estruturas em Memória (Sem gravar na BD a meio)
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
                // Se já existem, vamos apenas buscá-las à BD para associar
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

            // Equipas (Usamos a propriedade de navegação em vez de forçar o ID logo à partida)
            if (!await dbContext.Teams.AnyAsync() && cs2Category != null && lolCategory != null)
            {
                var cs2 = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "CS2");
                var lol = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "LOL");

                if (cs2 != null && lol != null)
                {
                    new Team { Name = "Natus Vincere", LogoPath = "navi.png", IsManualOverride = true, Category = cs2Category },
                    new Team { Name = "T1 Esports", LogoPath = "t1.png", IsManualOverride = true, Category = lolCategory },
                    new Team { Name = "Team Liquid", LogoPath = "liquid.png", IsManualOverride = true, Category = cs2Category }
                };
                await dbContext.Teams.AddRangeAsync(teams);
                haAdicao = true;
            }
        }

            // SaveChangesAsync no final para otimizar o I/O
            if (haAdicao)
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}