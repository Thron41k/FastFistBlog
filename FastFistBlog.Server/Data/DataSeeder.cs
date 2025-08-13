using FastFistBlog.Server.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace FastFistBlog.Server.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        string[] roles = ["Administrator", "Moderator", "User"];

        var roleDescriptions = new Dictionary<string, string>
        {
            { "Administrator", "Полные права на управление системой" },
            { "Moderator", "Может модерировать контент" },
            { "User", "Обычный пользователь с доступом к базовым функциям" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var newRole = new ApplicationRole
                {
                    Name = role,
                    Description = roleDescriptions[role]
                };

                await roleManager.CreateAsync(newRole);
            }
        }

        // Админ
        var admin = await userManager.FindByEmailAsync("admin@test.com");
        if (admin == null)
        {
            admin = new ApplicationUser { UserName = "admin@test.com", Email = "admin@test.com" };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Administrator");
        }

        // Модератор
        var mod = await userManager.FindByEmailAsync("mod@test.com");
        if (mod == null)
        {
            mod = new ApplicationUser { UserName = "mod@test.com", Email = "mod@test.com" };
            await userManager.CreateAsync(mod, "Mod123!");
            await userManager.AddToRoleAsync(mod, "Moderator");
        }

        // Пользователь
        var user = await userManager.FindByEmailAsync("user@test.com");
        if (user == null)
        {
            user = new ApplicationUser { UserName = "user@test.com", Email = "user@test.com" };
            await userManager.CreateAsync(user, "User123!");
            await userManager.AddToRoleAsync(user, "User");
        }
    }
}

