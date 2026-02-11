using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrackPoint.Configuration;

namespace TrackPoint.Data
{
    public static class AdminSeedData // Renamed from SeedData
    {
        private const string AdminRoleName = "Admin";

        public static async Task SeedAdminsAsync(IServiceProvider services)
        {
            var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;

            if (options.AdminEmails == null || options.AdminEmails.Count == 0)
            {
                return;
            }

            // Ensure Admin role exists
            if (!await roleManager.Roles.AnyAsync(r => r.Name == AdminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
            }

            foreach (var email in options.AdminEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                var normalizedEmail = email.Trim();

                var user = await userManager.FindByEmailAsync(normalizedEmail);
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = normalizedEmail,
                        Email = normalizedEmail,
                        EmailConfirmed = true
                    };

                    // Choose a default password pattern that you’ll change later in production
                    var createResult = await userManager.CreateAsync(user, "Admin#1234");
                    if (!createResult.Succeeded)
                    {
                        // Optionally log errors here
                        continue;
                    }
                }

                // Ensure user is in Admin role
                if (!await userManager.IsInRoleAsync(user, AdminRoleName))
                {
                    await userManager.AddToRoleAsync(user, AdminRoleName);
                }
            }
        }
    }
}