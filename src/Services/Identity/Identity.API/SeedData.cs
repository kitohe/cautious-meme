using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.API
{
    public static class SeedData
    {
        public static async Task SeedUserRoles(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = configuration["SystemUserRoles"]
                .Split(',', StringSplitOptions.TrimEntries).ToList();

            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);

                if (!roleExist)
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public static async Task SeedAdminAccount(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(configuration["AdminUser:UserEmail"]);
            if (user != null) return;

            var adminUser = new ApplicationUser
            {
                UserName = configuration["AdminUser:UserName"],
                Email = configuration["AdminUser:UserEmail"],
                FirstName = configuration["AdminUser:FirstName"],
                LastName = configuration["AdminUser:LastName"],
                Country = configuration["AdminUser:UserCountry"]
            };

            var adminUserPassword = configuration["AdminUser:UserPassword"];

            var createPowerUser = await userManager.CreateAsync(adminUser, adminUserPassword);
            if (createPowerUser.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
