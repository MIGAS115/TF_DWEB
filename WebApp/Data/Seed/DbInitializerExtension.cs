using ESports.Domain.Data;
using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Data.Seed
{

    internal static class DbInitializerExtension
    {

        public static async Task UseItToSeedSqlServerAsync(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<MyUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await DbInitializer.InitializeAsync(context, userManager, roleManager);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}