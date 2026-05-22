using Microsoft.EntityFrameworkCore;
using ZapateriaMR.Domain.Entities;

namespace ZapateriaMR.Infrastructure.Data.Seed;

public static class CategoriaProductoSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        bool existenCategorias = await context.CategoriasProducto.AnyAsync();

        if (existenCategorias)
        {
            return;
        }

        var categorias = new List<CategoriaProducto>
        {
            new()
            {
                Nombre = "Calzado deportivo",
                Descripcion = "Zapatos deportivos para uso diario, entrenamiento o actividades físicas."
            },
            new()
            {
                Nombre = "Calzado casual",
                Descripcion = "Zapatos casuales para uso cotidiano."
            },
            new()
            {
                Nombre = "Calzado formal",
                Descripcion = "Zapatos formales para eventos, trabajo u ocasiones especiales."
            },
            new()
            {
                Nombre = "Sandalias",
                Descripcion = "Sandalias y calzado abierto."
            },
            new()
            {
                Nombre = "Botas",
                Descripcion = "Botas para uso casual, trabajo o temporada."
            }
        };

        context.CategoriasProducto.AddRange(categorias);

        await context.SaveChangesAsync();
    }
}