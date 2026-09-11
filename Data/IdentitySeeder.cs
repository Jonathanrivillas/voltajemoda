using Microsoft.AspNetCore.Identity;

namespace VoltajeModa.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        string[] roles = ["Cliente", "Administrador"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        const string adminEmail = "admin@voltajemoda.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nombre = "Administrador Voltaje Moda",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, "Admin123!");
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
        {
            await userManager.AddToRoleAsync(adminUser, "Administrador");
        }
    }
}
