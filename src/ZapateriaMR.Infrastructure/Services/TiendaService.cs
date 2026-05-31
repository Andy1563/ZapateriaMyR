using Microsoft.EntityFrameworkCore;
using ZapateriaMR.Application.DTOs.Tienda;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Infrastructure.Data;

namespace ZapateriaMR.Infrastructure.Services;

public class TiendaService : ITiendaService
{
    private readonly ApplicationDbContext _context;

    public TiendaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TiendaProductoListadoDto>> ObtenerProductosAsync(
        string? busqueda = null,
        int? categoriaId = null)
    {
        var query = _context.Productos
            .AsNoTracking()
            .Include(p => p.CategoriaProducto)
            .Include(p => p.Inventario)
            .Where(p =>
                p.Activo &&
                !p.EstaEliminado &&
                p.Inventario != null &&
                p.Inventario.CantidadDisponible > 0);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();

            query = query.Where(p =>
                p.Nombre.Contains(termino) ||
                p.CodigoSku.Contains(termino) ||
                (p.Marca != null && p.Marca.Contains(termino)) ||
                (p.Color != null && p.Color.Contains(termino)) ||
                (p.Talla != null && p.Talla.Contains(termino)));
        }

        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            query = query.Where(p => p.CategoriaProductoId == categoriaId.Value);
        }

        return await query
            .OrderBy(p => p.Nombre)
            .Select(p => new TiendaProductoListadoDto
            {
                Id = p.Id,
                CodigoSku = p.CodigoSku,
                Nombre = p.Nombre,
                Marca = p.Marca,
                Color = p.Color,
                Talla = p.Talla,
                ImagenUrl = p.ImagenUrl,
                Categoria = p.CategoriaProducto != null
                    ? p.CategoriaProducto.Nombre
                    : "Sin categoría",
                PrecioVenta = p.PrecioVenta,
                CantidadDisponible = p.Inventario != null
                    ? p.Inventario.CantidadDisponible
                    : 0
            })
            .ToListAsync();
    }

    public async Task<TiendaProductoDetalleDto?> ObtenerProductoPorIdAsync(int id)
    {
        return await _context.Productos
            .AsNoTracking()
            .Include(p => p.CategoriaProducto)
            .Include(p => p.Inventario)
            .Where(p =>
                p.Id == id &&
                p.Activo &&
                !p.EstaEliminado &&
                p.Inventario != null &&
                p.Inventario.CantidadDisponible > 0)
            .Select(p => new TiendaProductoDetalleDto
            {
                Id = p.Id,
                CodigoSku = p.CodigoSku,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Marca = p.Marca,
                Color = p.Color,
                Talla = p.Talla,
                ImagenUrl = p.ImagenUrl,
                Categoria = p.CategoriaProducto != null
                    ? p.CategoriaProducto.Nombre
                    : "Sin categoría",
                PrecioVenta = p.PrecioVenta,
                CantidadDisponible = p.Inventario != null
                    ? p.Inventario.CantidadDisponible
                    : 0
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<CategoriaTiendaDto>> ObtenerCategoriasAsync()
    {
        return await _context.CategoriasProducto
            .AsNoTracking()
            .Where(c => c.Activo && !c.EstaEliminado)
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaTiendaDto
            {
                Id = c.Id,
                Nombre = c.Nombre
            })
            .ToListAsync();
    }
}