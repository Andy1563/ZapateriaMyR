using Microsoft.AspNetCore.Identity;
using ZapateriaMR.Infrastructure.Identity;

namespace ZapateriaMR.Infrastructure.Data.Seed;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        [
            "Administrador",
            "UsuarioDueño",
            "Cliente"
        ];

        foreach (var role in roles)
        {
            bool roleExists = await roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedAdminAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string nombre,
        string apellido)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is null)
        {
            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Nombre = nombre,
                Apellido = apellido,
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"No se pudo crear el usuario administrador inicial: {errors}");
            }

            await userManager.AddToRoleAsync(admin, "Administrador");

            return;
        }

        if (!await userManager.IsInRoleAsync(existingUser, "Administrador"))
        {
            await userManager.AddToRoleAsync(existingUser, "Administrador");
        }
    }
}