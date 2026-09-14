using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace VoltajeModa.Data;

public static class IdentitySeeder
{
    private static readonly string[] Roles = ["Cliente", "Administrador"];

    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    /// <summary>
    /// Crea un usuario administrador de prueba con contraseña conocida. Solo debe llamarse en
    /// entornos de desarrollo: en producción el administrador se crea manualmente con una
    /// contraseña fuerte y única.
    /// </summary>
    public static async Task SeedDevAdminAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
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

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (!result.Succeeded)
            {
                logger.LogError("No se pudo crear el usuario administrador de prueba: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
        {
            await userManager.AddToRoleAsync(adminUser, "Administrador");
        }
    }
}
