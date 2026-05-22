using Microsoft.EntityFrameworkCore;
using ZapateriaMR.Application.DTOs.Productos;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Entities;
using ZapateriaMR.Domain.Enums;
using ZapateriaMR.Infrastructure.Data;

namespace ZapateriaMR.Infrastructure.Services;

public class ProductoService : IProductoService
{
    private readonly ApplicationDbContext _context;

    public ProductoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductoListadoDto>> ObtenerTodosAsync(string? busqueda = null)
    {
        var query = _context.Productos
            .AsNoTracking()
            .Include(p => p.CategoriaProducto)
            .Include(p => p.Inventario)
            .Where(p => !p.EstaEliminado);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();

            query = query.Where(p =>
                p.CodigoSku.Contains(termino) ||
                p.Nombre.Contains(termino) ||
                (p.Marca != null && p.Marca.Contains(termino)));
        }

        return await query
            .OrderBy(p => p.Nombre)
            .Select(p => new ProductoListadoDto
            {
                Id = p.Id,
                CodigoSku = p.CodigoSku,
                Nombre = p.Nombre,
                Marca = p.Marca,
                Color = p.Color,
                Talla = p.Talla,
                Categoria = p.CategoriaProducto != null ? p.CategoriaProducto.Nombre : "Sin categoría",
                PrecioVenta = p.PrecioVenta,
                CantidadDisponible = p.Inventario != null ? p.Inventario.CantidadDisponible : 0,
                Activo = p.Activo
            })
            .ToListAsync();
    }

    public async Task<ProductoDetalleDto?> ObtenerPorIdAsync(int id)
    {
        return await _context.Productos
            .AsNoTracking()
            .Include(p => p.CategoriaProducto)
            .Include(p => p.Inventario)
            .Where(p => p.Id == id && !p.EstaEliminado)
            .Select(p => new ProductoDetalleDto
            {
                Id = p.Id,
                CodigoSku = p.CodigoSku,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Marca = p.Marca,
                Color = p.Color,
                Talla = p.Talla,
                Categoria = p.CategoriaProducto != null ? p.CategoriaProducto.Nombre : "Sin categoría",
                PrecioCompra = p.PrecioCompra,
                PrecioVenta = p.PrecioVenta,
                CantidadDisponible = p.Inventario != null ? p.Inventario.CantidadDisponible : 0,
                CantidadReservada = p.Inventario != null ? p.Inventario.CantidadReservada : 0,
                StockMinimo = p.Inventario != null ? p.Inventario.StockMinimo : 0,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion,
                FechaModificacion = p.FechaModificacion
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EditarProductoDto?> ObtenerParaEditarAsync(int id)
    {
        return await _context.Productos
            .AsNoTracking()
            .Include(p => p.Inventario)
            .Where(p => p.Id == id && !p.EstaEliminado)
            .Select(p => new EditarProductoDto
            {
                Id = p.Id,
                CodigoSku = p.CodigoSku,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Marca = p.Marca,
                Color = p.Color,
                Talla = p.Talla,
                PrecioCompra = p.PrecioCompra,
                PrecioVenta = p.PrecioVenta,
                CategoriaProductoId = p.CategoriaProductoId,
                StockMinimo = p.Inventario != null ? p.Inventario.StockMinimo : 0,
                Activo = p.Activo
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<CategoriaProductoSelectDto>> ObtenerCategoriasAsync()
    {
        return await _context.CategoriasProducto
            .AsNoTracking()
            .Where(c => !c.EstaEliminado && c.Activo)
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaProductoSelectDto
            {
                Id = c.Id,
                Nombre = c.Nombre
            })
            .ToListAsync();
    }

    public async Task<int> CrearAsync(CrearProductoDto dto, string? usuarioId = null)
    {
        await ValidarCategoriaAsync(dto.CategoriaProductoId);

        await ValidarSkuUnicoAsync(dto.CodigoSku);

        if (dto.PrecioCompra < 0 || dto.PrecioVenta < 0)
        {
            throw new InvalidOperationException("Los precios no pueden ser negativos.");
        }

        if (dto.CantidadInicial < 0 || dto.StockMinimo < 0)
        {
            throw new InvalidOperationException("Las cantidades de inventario no pueden ser negativas.");
        }

        var producto = new Producto
        {
            CodigoSku = dto.CodigoSku.Trim(),
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            Marca = dto.Marca?.Trim(),
            Color = dto.Color?.Trim(),
            Talla = dto.Talla?.Trim(),
            PrecioCompra = dto.PrecioCompra,
            PrecioVenta = dto.PrecioVenta,
            CategoriaProductoId = dto.CategoriaProductoId,
            UsuarioCreacionId = usuarioId,
            FechaCreacion = DateTime.UtcNow,
            Inventario = new Inventario
            {
                CantidadDisponible = dto.CantidadInicial,
                CantidadReservada = 0,
                StockMinimo = dto.StockMinimo,
                FechaUltimaActualizacion = DateTime.UtcNow,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            }
        };

        _context.Productos.Add(producto);

        await _context.SaveChangesAsync();

        if (dto.CantidadInicial > 0)
        {
            var movimiento = new MovimientoInventario
            {
                ProductoId = producto.Id,
                TipoMovimiento = TipoMovimientoInventario.Entrada,
                Cantidad = dto.CantidadInicial,
                StockAnterior = 0,
                StockNuevo = dto.CantidadInicial,
                Motivo = "Inventario inicial del producto",
                UsuarioId = usuarioId,
                UsuarioCreacionId = usuarioId,
                FechaMovimiento = DateTime.UtcNow,
                FechaCreacion = DateTime.UtcNow
            };

            _context.MovimientosInventario.Add(movimiento);

            await _context.SaveChangesAsync();
        }

        return producto.Id;
    }

    public async Task<bool> EditarAsync(EditarProductoDto dto, string? usuarioId = null)
    {
        var producto = await _context.Productos
            .Include(p => p.Inventario)
            .FirstOrDefaultAsync(p => p.Id == dto.Id && !p.EstaEliminado);

        if (producto is null)
        {
            return false;
        }

        await ValidarCategoriaAsync(dto.CategoriaProductoId);

        await ValidarSkuUnicoAsync(dto.CodigoSku, dto.Id);

        if (dto.PrecioCompra < 0 || dto.PrecioVenta < 0)
        {
            throw new InvalidOperationException("Los precios no pueden ser negativos.");
        }

        if (dto.StockMinimo < 0)
        {
            throw new InvalidOperationException("El stock mínimo no puede ser negativo.");
        }

        producto.CodigoSku = dto.CodigoSku.Trim();
        producto.Nombre = dto.Nombre.Trim();
        producto.Descripcion = dto.Descripcion?.Trim();
        producto.Marca = dto.Marca?.Trim();
        producto.Color = dto.Color?.Trim();
        producto.Talla = dto.Talla?.Trim();
        producto.PrecioCompra = dto.PrecioCompra;
        producto.PrecioVenta = dto.PrecioVenta;
        producto.CategoriaProductoId = dto.CategoriaProductoId;
        producto.Activo = dto.Activo;
        producto.FechaModificacion = DateTime.UtcNow;
        producto.UsuarioModificacionId = usuarioId;

        if (producto.Inventario is null)
        {
            producto.Inventario = new Inventario
            {
                ProductoId = producto.Id,
                CantidadDisponible = 0,
                CantidadReservada = 0,
                StockMinimo = dto.StockMinimo,
                FechaUltimaActualizacion = DateTime.UtcNow,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };
        }
        else
        {
            producto.Inventario.StockMinimo = dto.StockMinimo;
            producto.Inventario.FechaUltimaActualizacion = DateTime.UtcNow;
            producto.Inventario.FechaModificacion = DateTime.UtcNow;
            producto.Inventario.UsuarioModificacionId = usuarioId;
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DesactivarAsync(int id, string? usuarioId = null)
    {
        var producto = await _context.Productos
            .FirstOrDefaultAsync(p => p.Id == id && !p.EstaEliminado);

        if (producto is null)
        {
            return false;
        }

        producto.Activo = false;
        producto.EstaEliminado = true;
        producto.FechaModificacion = DateTime.UtcNow;
        producto.UsuarioModificacionId = usuarioId;

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task ValidarCategoriaAsync(int categoriaProductoId)
    {
        var categoriaExiste = await _context.CategoriasProducto
            .AnyAsync(c => c.Id == categoriaProductoId && !c.EstaEliminado && c.Activo);

        if (!categoriaExiste)
        {
            throw new InvalidOperationException("La categoría seleccionada no existe o está inactiva.");
        }
    }

    private async Task ValidarSkuUnicoAsync(string codigoSku, int? productoIdExcluir = null)
    {
        var codigoNormalizado = codigoSku.Trim();

        var skuExiste = await _context.Productos
            .AnyAsync(p =>
                p.CodigoSku == codigoNormalizado &&
                !p.EstaEliminado &&
                (!productoIdExcluir.HasValue || p.Id != productoIdExcluir.Value));

        if (skuExiste)
        {
            throw new InvalidOperationException("Ya existe un producto con el mismo código SKU.");
        }
    }
}