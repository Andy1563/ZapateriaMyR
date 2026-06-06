using Microsoft.EntityFrameworkCore;
using ZapateriaMR.Application.DTOs.Reportes;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Enums;
using ZapateriaMR.Infrastructure.Data;

namespace ZapateriaMR.Infrastructure.Services;

public class ReporteService : IReporteService
{
    private readonly ApplicationDbContext _context;

    public ReporteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReporteResumenDto> ObtenerResumenAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var pedidosQuery = _context.Pedidos
            .AsNoTracking()
            .Where(p => !p.EstaEliminado);

        if (fechaInicio.HasValue)
        {
            var inicio = fechaInicio.Value.Date;
            pedidosQuery = pedidosQuery.Where(p => p.FechaPedido >= inicio);
        }

        if (fechaFin.HasValue)
        {
            var fin = fechaFin.Value.Date.AddDays(1);
            pedidosQuery = pedidosQuery.Where(p => p.FechaPedido < fin);
        }

        var pedidosNoCancelados = pedidosQuery
            .Where(p => p.Estado != EstadoPedido.Cancelado);

        var totalProductosActivos = await _context.Productos
            .AsNoTracking()
            .CountAsync(p => p.Activo && !p.EstaEliminado);

        var totalUnidadesInventario = await _context.Inventarios
            .AsNoTracking()
            .Where(i => i.Producto != null && i.Producto.Activo && !i.Producto.EstaEliminado)
            .SumAsync(i => (int?)i.CantidadDisponible) ?? 0;

        var productosStockBajo = await _context.Inventarios
            .AsNoTracking()
            .Where(i =>
                i.Producto != null &&
                i.Producto.Activo &&
                !i.Producto.EstaEliminado &&
                i.CantidadDisponible <= i.StockMinimo)
            .CountAsync();

        var pedidosPendientes = await pedidosQuery
            .CountAsync(p => p.Estado == EstadoPedido.Pendiente);

        var pedidosEntregados = await pedidosQuery
            .CountAsync(p => p.Estado == EstadoPedido.Entregado);

        var pedidosCancelados = await pedidosQuery
            .CountAsync(p => p.Estado == EstadoPedido.Cancelado);

        var totalVendido = await pedidosNoCancelados
            .SumAsync(p => (decimal?)p.Total) ?? 0;

        var pedidosPorEstado = await pedidosQuery
            .GroupBy(p => p.Estado)
            .Select(g => new ReportePedidoPorEstadoDto
            {
                Estado = g.Key,
                Cantidad = g.Count(),
                Total = g.Sum(p => p.Total)
            })
            .ToListAsync();

        var productosMasVendidos = await pedidosNoCancelados
            .SelectMany(p => p.Detalles)
            .Where(d => d.Producto != null)
            .GroupBy(d => new
            {
                d.ProductoId,
                d.Producto!.CodigoSku,
                d.Producto.Nombre
            })
            .Select(g => new ReporteProductoMasVendidoDto
            {
                ProductoId = g.Key.ProductoId,
                CodigoSku = g.Key.CodigoSku,
                NombreProducto = g.Key.Nombre,
                UnidadesVendidas = g.Sum(d => d.Cantidad),
                TotalVendido = g.Sum(d => d.Subtotal)
            })
            .OrderByDescending(p => p.UnidadesVendidas)
            .ThenByDescending(p => p.TotalVendido)
            .Take(10)
            .ToListAsync();

        var productosConStockBajo = await _context.Inventarios
            .AsNoTracking()
            .Where(i =>
                i.Producto != null &&
                i.Producto.Activo &&
                !i.Producto.EstaEliminado &&
                i.CantidadDisponible <= i.StockMinimo)
            .OrderBy(i => i.CantidadDisponible)
            .ThenBy(i => i.Producto!.Nombre)
            .Select(i => new ReporteStockBajoDto
            {
                ProductoId = i.ProductoId,
                CodigoSku = i.Producto!.CodigoSku,
                NombreProducto = i.Producto.Nombre,
                ImagenUrl = i.Producto.ImagenUrl,
                CantidadDisponible = i.CantidadDisponible,
                StockMinimo = i.StockMinimo
            })
            .Take(10)
            .ToListAsync();

        var pedidosRecientes = await pedidosQuery
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => new ReportePedidoRecienteDto
            {
                PedidoId = p.Id,
                NumeroPedido = p.NumeroPedido,
                NombreCliente = p.NombreCliente,
                Estado = p.Estado,
                Total = p.Total,
                FechaPedido = p.FechaPedido
            })
            .Take(10)
            .ToListAsync();

        return new ReporteResumenDto
        {
            TotalProductosActivos = totalProductosActivos,
            TotalUnidadesInventario = totalUnidadesInventario,
            ProductosStockBajo = productosStockBajo,
            PedidosPendientes = pedidosPendientes,
            PedidosEntregados = pedidosEntregados,
            PedidosCancelados = pedidosCancelados,
            TotalVendido = totalVendido,
            PedidosPorEstado = pedidosPorEstado,
            ProductosMasVendidos = productosMasVendidos,
            ProductosConStockBajo = productosConStockBajo,
            PedidosRecientes = pedidosRecientes
        };
    }
}