using Events.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Events.WebApi
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            const string adminUserName = "admin";
            const string adminPassword = "Admin1"; 

            if (await userManager.FindByNameAsync(adminUserName) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, adminRole);
                }
                else
                {
                    throw new Exception($"Не удалось создать админа: {string.Join(", ", result.Errors)}");
                }
            }
        }
    }
}
