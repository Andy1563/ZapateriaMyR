using Microsoft.EntityFrameworkCore;
using ZapateriaMR.Application.DTOs.Inventario;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Entities;
using ZapateriaMR.Domain.Enums;
using ZapateriaMR.Infrastructure.Data;
using ZapateriaMR.Application.DTOs.Auditoria;

namespace ZapateriaMR.Infrastructure.Services;

public class InventarioService : IInventarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditoriaService _auditoriaService;

    public InventarioService(
        ApplicationDbContext context,
        IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<IReadOnlyList<InventarioListadoDto>> ObtenerInventarioAsync(string? busqueda = null)
    {
        var query = _context.Inventarios
            .AsNoTracking()
            .Include(i => i.Producto)
                .ThenInclude(p => p!.CategoriaProducto)
            .Where(i =>
                i.Producto != null &&
                !i.Producto.EstaEliminado);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();

            query = query.Where(i =>
                i.Producto!.CodigoSku.Contains(termino) ||
                i.Producto.Nombre.Contains(termino) ||
                (i.Producto.Marca != null && i.Producto.Marca.Contains(termino)));
        }

        return await query
            .OrderBy(i => i.Producto!.Nombre)
            .Select(i => new InventarioListadoDto
            {
                ProductoId = i.ProductoId,
                CodigoSku = i.Producto!.CodigoSku,
                NombreProducto = i.Producto.Nombre,
                Marca = i.Producto.Marca,
                Color = i.Producto.Color,
                Talla = i.Producto.Talla,
                ImagenUrl = i.Producto.ImagenUrl,
                Categoria = i.Producto.CategoriaProducto != null
                    ? i.Producto.CategoriaProducto.Nombre
                    : "Sin categoría",
                CantidadDisponible = i.CantidadDisponible,
                CantidadReservada = i.CantidadReservada,
                StockMinimo = i.StockMinimo,
                FechaUltimaActualizacion = i.FechaUltimaActualizacion
            })
            .ToListAsync();
    }

    public async Task<InventarioDetalleDto?> ObtenerDetallePorProductoIdAsync(int productoId)
    {
        var inventario = await _context.Inventarios
            .AsNoTracking()
            .Include(i => i.Producto)
                .ThenInclude(p => p!.CategoriaProducto)
            .FirstOrDefaultAsync(i =>
                i.ProductoId == productoId &&
                i.Producto != null &&
                !i.Producto.EstaEliminado);

        if (inventario is null || inventario.Producto is null)
        {
            return null;
        }

        var movimientos = await _context.MovimientosInventario
            .AsNoTracking()
            .Where(m => m.ProductoId == productoId && !m.EstaEliminado)
            .OrderByDescending(m => m.FechaMovimiento)
            .Select(m => new MovimientoInventarioDto
            {
                Id = m.Id,
                TipoMovimiento = m.TipoMovimiento,
                Cantidad = m.Cantidad,
                StockAnterior = m.StockAnterior,
                StockNuevo = m.StockNuevo,
                Motivo = m.Motivo,
                UsuarioId = m.UsuarioId,
                FechaMovimiento = m.FechaMovimiento
            })
            .ToListAsync();

        return new InventarioDetalleDto
        {
            ProductoId = inventario.ProductoId,
            CodigoSku = inventario.Producto.CodigoSku,
            NombreProducto = inventario.Producto.Nombre,
            Marca = inventario.Producto.Marca,
            Color = inventario.Producto.Color,
            Talla = inventario.Producto.Talla,
            ImagenUrl = inventario.Producto.ImagenUrl,
            Categoria = inventario.Producto.CategoriaProducto != null
                ? inventario.Producto.CategoriaProducto.Nombre
                : "Sin categoría",
            CantidadDisponible = inventario.CantidadDisponible,
            CantidadReservada = inventario.CantidadReservada,
            StockMinimo = inventario.StockMinimo,
            FechaUltimaActualizacion = inventario.FechaUltimaActualizacion,
            Movimientos = movimientos
        };
    }

    public async Task<bool> RegistrarMovimientoAsync(RegistrarMovimientoInventarioDto dto, string? usuarioId = null)
    {
        if (dto.Cantidad < 0)
        {
            throw new InvalidOperationException("La cantidad no puede ser negativa.");
        }

        var producto = await _context.Productos
            .Include(p => p.Inventario)
            .FirstOrDefaultAsync(p =>
                p.Id == dto.ProductoId &&
                !p.EstaEliminado);

        if (producto is null)
        {
            return false;
        }

        if (producto.Inventario is null)
        {
            producto.Inventario = new Inventario
            {
                ProductoId = producto.Id,
                CantidadDisponible = 0,
                CantidadReservada = 0,
                StockMinimo = 0,
                FechaUltimaActualizacion = DateTime.UtcNow,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow
            };
        }

        var stockAnterior = producto.Inventario.CantidadDisponible;
        int stockNuevo;
        int cantidadMovimiento;

        switch (dto.TipoMovimiento)
        {
            case TipoMovimientoInventario.Entrada:
                if (dto.Cantidad <= 0)
                {
                    throw new InvalidOperationException("La entrada debe ser mayor a cero.");
                }

                stockNuevo = stockAnterior + dto.Cantidad;
                cantidadMovimiento = dto.Cantidad;
                break;

            case TipoMovimientoInventario.Salida:
                if (dto.Cantidad <= 0)
                {
                    throw new InvalidOperationException("La salida debe ser mayor a cero.");
                }

                if (dto.Cantidad > stockAnterior)
                {
                    throw new InvalidOperationException("No hay suficiente stock disponible para realizar la salida.");
                }

                stockNuevo = stockAnterior - dto.Cantidad;
                cantidadMovimiento = dto.Cantidad;
                break;

            case TipoMovimientoInventario.Ajuste:
                stockNuevo = dto.Cantidad;
                cantidadMovimiento = Math.Abs(stockNuevo - stockAnterior);
                break;

            default:
                throw new InvalidOperationException("Tipo de movimiento de inventario no válido.");
        }

        producto.Inventario.CantidadDisponible = stockNuevo;
        producto.Inventario.FechaUltimaActualizacion = DateTime.UtcNow;
        producto.Inventario.FechaModificacion = DateTime.UtcNow;
        producto.Inventario.UsuarioModificacionId = usuarioId;

        var movimiento = new MovimientoInventario
        {
            ProductoId = producto.Id,
            TipoMovimiento = dto.TipoMovimiento,
            Cantidad = cantidadMovimiento,
            StockAnterior = stockAnterior,
            StockNuevo = stockNuevo,
            Motivo = dto.Motivo?.Trim(),
            UsuarioId = usuarioId,
            UsuarioCreacionId = usuarioId,
            FechaMovimiento = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow
        };

        _context.MovimientosInventario.Add(movimiento);

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(new RegistrarAuditoriaDto
        {
            UsuarioId = usuarioId,
            Accion = TipoAccionAuditoria.Editar,
            EntidadAfectada = "Inventario",
            RegistroId = producto.Id.ToString(),
            Detalle = $"Se registró un movimiento de inventario tipo '{dto.TipoMovimiento}' para el producto '{producto.Nombre}'. Stock anterior: {stockAnterior}. Stock nuevo: {stockNuevo}."
        });

        return true;
    }
}