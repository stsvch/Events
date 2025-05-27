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

            // 1. Создаём роль Admin, если её ещё нет
            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // 2. Создаём пользователя admin, если его нет
            const string adminUserName = "admin";
            const string adminPassword = "Admin1"; // отвечает требованиям: 6+ символов, есть Upper, Lower, Digit

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
                    // 3. Добавляем пользователя в роль Admin
                    await userManager.AddToRoleAsync(admin, adminRole);
                }
                else
                {
                    // на продакшене логируйте ошибки result.Errors
                    throw new Exception($"Не удалось создать админа: {string.Join(", ", result.Errors)}");
                }
            }
        }
    }
}
