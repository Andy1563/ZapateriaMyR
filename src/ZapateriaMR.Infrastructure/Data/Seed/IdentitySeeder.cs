using Microsoft.AspNetCore.Identity;

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
}