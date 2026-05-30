using Microsoft.EntityFrameworkCore;
using ZapateriaMR.Application.DTOs.Pedidos;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Entities;
using ZapateriaMR.Domain.Enums;
using ZapateriaMR.Infrastructure.Data;
using ZapateriaMR.Application.DTOs.Auditoria;

namespace ZapateriaMR.Infrastructure.Services;

public class PedidoService : IPedidoService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditoriaService _auditoriaService;

    public PedidoService(
        ApplicationDbContext context,
        IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<IReadOnlyList<PedidoListadoDto>> ObtenerPedidosAsync(string? busqueda = null)
    {
        var query = _context.Pedidos
            .AsNoTracking()
            .Include(p => p.Detalles)
            .Where(p => !p.EstaEliminado);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();

            query = query.Where(p =>
                p.NumeroPedido.Contains(termino) ||
                p.NombreCliente.Contains(termino) ||
                (p.CorreoCliente != null && p.CorreoCliente.Contains(termino)) ||
                (p.TelefonoCliente != null && p.TelefonoCliente.Contains(termino)));
        }

        return await query
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => new PedidoListadoDto
            {
                Id = p.Id,
                NumeroPedido = p.NumeroPedido,
                NombreCliente = p.NombreCliente,
                Estado = p.Estado,
                Total = p.Total,
                CantidadProductos = p.Detalles.Sum(d => d.Cantidad),
                FechaPedido = p.FechaPedido,
                FechaEntregaEstimada = p.FechaEntregaEstimada
            })
            .ToListAsync();
    }

    public async Task<PedidoDetalleDto?> ObtenerPorIdAsync(int id)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.Id == id && !p.EstaEliminado)
            .Select(p => new PedidoDetalleDto
            {
                Id = p.Id,
                NumeroPedido = p.NumeroPedido,
                NombreCliente = p.NombreCliente,
                CorreoCliente = p.CorreoCliente,
                TelefonoCliente = p.TelefonoCliente,
                DireccionEntrega = p.DireccionEntrega,
                Estado = p.Estado,
                Subtotal = p.Subtotal,
                Total = p.Total,
                FechaPedido = p.FechaPedido,
                FechaEntregaEstimada = p.FechaEntregaEstimada,
                Observaciones = p.Observaciones,
                Detalles = p.Detalles.Select(d => new PedidoDetalleLineaDto
                {
                    ProductoId = d.ProductoId,
                    CodigoSku = d.Producto != null ? d.Producto.CodigoSku : string.Empty,
                    NombreProducto = d.Producto != null ? d.Producto.Nombre : "Producto no disponible",
                    ImagenUrl = d.Producto != null ? d.Producto.ImagenUrl : null,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<ProductoPedidoSelectDto>> ObtenerProductosDisponiblesAsync()
    {
        return await _context.Productos
            .AsNoTracking()
            .Include(p => p.Inventario)
            .Where(p =>
                p.Activo &&
                !p.EstaEliminado &&
                p.Inventario != null &&
                p.Inventario.CantidadDisponible > 0)
            .OrderBy(p => p.Nombre)
            .Select(p => new ProductoPedidoSelectDto
            {
                ProductoId = p.Id,
                CodigoSku = p.CodigoSku,
                Nombre = p.Nombre,
                Marca = p.Marca,
                Color = p.Color,
                Talla = p.Talla,
                ImagenUrl = p.ImagenUrl,
                PrecioVenta = p.PrecioVenta,
                CantidadDisponible = p.Inventario != null ? p.Inventario.CantidadDisponible : 0
            })
            .ToListAsync();
    }

    public async Task<int> CrearAsync(CrearPedidoDto dto, string? usuarioId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.NombreCliente))
        {
            throw new InvalidOperationException("El nombre del cliente es obligatorio.");
        }

        var detallesValidos = dto.Detalles
            .Where(d => d.ProductoId > 0 && d.Cantidad > 0)
            .GroupBy(d => d.ProductoId)
            .Select(g => new CrearDetallePedidoDto
            {
                ProductoId = g.Key,
                Cantidad = g.Sum(x => x.Cantidad)
            })
            .ToList();

        if (!detallesValidos.Any())
        {
            throw new InvalidOperationException("El pedido debe tener al menos un producto.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var productoIds = detallesValidos.Select(d => d.ProductoId).ToList();

        var productos = await _context.Productos
            .Include(p => p.Inventario)
            .Where(p =>
                productoIds.Contains(p.Id) &&
                p.Activo &&
                !p.EstaEliminado)
            .ToListAsync();

        var pedido = new Pedido
        {
            NumeroPedido = GenerarNumeroPedido(),
            NombreCliente = dto.NombreCliente.Trim(),
            CorreoCliente = dto.CorreoCliente?.Trim(),
            TelefonoCliente = dto.TelefonoCliente?.Trim(),
            DireccionEntrega = dto.DireccionEntrega?.Trim(),
            FechaEntregaEstimada = dto.FechaEntregaEstimada,
            Observaciones = dto.Observaciones?.Trim(),
            Estado = EstadoPedido.Pendiente,
            FechaPedido = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow,
            UsuarioCreacionId = usuarioId
        };

        decimal subtotalPedido = 0;

        foreach (var detalleDto in detallesValidos)
        {
            var producto = productos.FirstOrDefault(p => p.Id == detalleDto.ProductoId);

            if (producto is null)
            {
                throw new InvalidOperationException("Uno de los productos seleccionados no existe o está inactivo.");
            }

            if (producto.Inventario is null)
            {
                throw new InvalidOperationException($"El producto {producto.Nombre} no tiene inventario configurado.");
            }

            if (producto.Inventario.CantidadDisponible < detalleDto.Cantidad)
            {
                throw new InvalidOperationException($"No hay suficiente stock disponible para el producto {producto.Nombre}.");
            }

            var stockAnterior = producto.Inventario.CantidadDisponible;
            var stockNuevo = stockAnterior - detalleDto.Cantidad;

            producto.Inventario.CantidadDisponible = stockNuevo;
            producto.Inventario.FechaUltimaActualizacion = DateTime.UtcNow;
            producto.Inventario.FechaModificacion = DateTime.UtcNow;
            producto.Inventario.UsuarioModificacionId = usuarioId;

            var precioUnitario = producto.PrecioVenta;
            var subtotalLinea = precioUnitario * detalleDto.Cantidad;

            subtotalPedido += subtotalLinea;

            pedido.Detalles.Add(new DetallePedido
            {
                ProductoId = producto.Id,
                Cantidad = detalleDto.Cantidad,
                PrecioUnitario = precioUnitario,
                Subtotal = subtotalLinea,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacionId = usuarioId
            });

            _context.MovimientosInventario.Add(new MovimientoInventario
            {
                ProductoId = producto.Id,
                TipoMovimiento = TipoMovimientoInventario.Salida,
                Cantidad = detalleDto.Cantidad,
                StockAnterior = stockAnterior,
                StockNuevo = stockNuevo,
                Motivo = $"Salida por creación del pedido {pedido.NumeroPedido}",
                UsuarioId = usuarioId,
                UsuarioCreacionId = usuarioId,
                FechaMovimiento = DateTime.UtcNow,
                FechaCreacion = DateTime.UtcNow
            });
        }

        pedido.Subtotal = subtotalPedido;
        pedido.Total = subtotalPedido;

        _context.Pedidos.Add(pedido);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        await _auditoriaService.RegistrarAsync(new RegistrarAuditoriaDto
        {
            UsuarioId = usuarioId,
            Accion = TipoAccionAuditoria.Crear,
            EntidadAfectada = "Pedido",
            RegistroId = pedido.Id.ToString(),
            Detalle = $"Se creó el pedido '{pedido.NumeroPedido}' para el cliente '{pedido.NombreCliente}' por un total de ₡{pedido.Total:N2}."
        });

        return pedido.Id;
    }

    public async Task<bool> CambiarEstadoAsync(int pedidoId, EstadoPedido nuevoEstado, string? usuarioId = null)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.Id == pedidoId && !p.EstaEliminado);

        if (pedido is null)
        {
            return false;
        }

        if (pedido.Estado == nuevoEstado)
        {
            return true;
        }

        if (pedido.Estado == EstadoPedido.Cancelado)
        {
            throw new InvalidOperationException("No se puede modificar un pedido cancelado.");
        }

        if (pedido.Estado == EstadoPedido.Entregado && nuevoEstado == EstadoPedido.Cancelado)
        {
            throw new InvalidOperationException("No se puede cancelar un pedido que ya fue entregado.");
        }

        if (nuevoEstado == EstadoPedido.Cancelado)
        {
            await DevolverStockPorCancelacionAsync(pedido, usuarioId);
        }

        var estadoAnterior = pedido.Estado;

        pedido.Estado = nuevoEstado;
        pedido.FechaModificacion = DateTime.UtcNow;
        pedido.UsuarioModificacionId = usuarioId;

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(new RegistrarAuditoriaDto
        {
            UsuarioId = usuarioId,
            Accion = nuevoEstado == EstadoPedido.Cancelado
                ? TipoAccionAuditoria.Eliminar
                : TipoAccionAuditoria.Editar,
            EntidadAfectada = "Pedido",
            RegistroId = pedido.Id.ToString(),
            Detalle = $"Se cambió el estado del pedido '{pedido.NumeroPedido}' de '{estadoAnterior}' a '{nuevoEstado}'."
        });

        return true;
    }

    private async Task DevolverStockPorCancelacionAsync(Pedido pedido, string? usuarioId)
    {
        var productoIds = pedido.Detalles.Select(d => d.ProductoId).ToList();

        var productos = await _context.Productos
            .Include(p => p.Inventario)
            .Where(p => productoIds.Contains(p.Id))
            .ToListAsync();

        foreach (var detalle in pedido.Detalles)
        {
            var producto = productos.FirstOrDefault(p => p.Id == detalle.ProductoId);

            if (producto?.Inventario is null)
            {
                continue;
            }

            var stockAnterior = producto.Inventario.CantidadDisponible;
            var stockNuevo = stockAnterior + detalle.Cantidad;

            producto.Inventario.CantidadDisponible = stockNuevo;
            producto.Inventario.FechaUltimaActualizacion = DateTime.UtcNow;
            producto.Inventario.FechaModificacion = DateTime.UtcNow;
            producto.Inventario.UsuarioModificacionId = usuarioId;

            _context.MovimientosInventario.Add(new MovimientoInventario
            {
                ProductoId = producto.Id,
                TipoMovimiento = TipoMovimientoInventario.Entrada,
                Cantidad = detalle.Cantidad,
                StockAnterior = stockAnterior,
                StockNuevo = stockNuevo,
                Motivo = $"Devolución de stock por cancelación del pedido {pedido.NumeroPedido}",
                UsuarioId = usuarioId,
                UsuarioCreacionId = usuarioId,
                FechaMovimiento = DateTime.UtcNow,
                FechaCreacion = DateTime.UtcNow
            });
        }
    }

    private static string GenerarNumeroPedido()
    {
        return $"PED-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }
}