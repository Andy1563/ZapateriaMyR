using Microsoft.AspNetCore.Mvc;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Enums;
using ZapateriaMR.Web.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;

namespace ZapateriaMR.Web.Controllers;

[Authorize(Roles = "Administrador,UsuarioDueño")]
public class HomeController : Controller
{
    private readonly IProductoService _productoService;
    private readonly IInventarioService _inventarioService;
    private readonly IPedidoService _pedidoService;
    private readonly IAuditoriaService _auditoriaService;

    public HomeController(
        IProductoService productoService,
        IInventarioService inventarioService,
        IPedidoService pedidoService,
        IAuditoriaService auditoriaService)
    {
        _productoService = productoService;
        _inventarioService = inventarioService;
        _pedidoService = pedidoService;
        _auditoriaService = auditoriaService;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.ObtenerTodosAsync();
        var inventario = await _inventarioService.ObtenerInventarioAsync();
        var pedidos = await _pedidoService.ObtenerPedidosAsync();
        var auditorias = await _auditoriaService.ObtenerAuditoriasAsync();

        var pedidosNoCancelados = pedidos
            .Where(p => p.Estado != EstadoPedido.Cancelado)
            .ToList();

        var viewModel = new DashboardViewModel
        {
            TotalProductos = productos.Count(p => p.Activo),
            ProductosStockBajo = inventario.Count(i => i.StockBajo),
            PedidosPendientes = pedidos.Count(p => p.Estado == EstadoPedido.Pendiente),
            PedidosCancelados = pedidos.Count(p => p.Estado == EstadoPedido.Cancelado),
            TotalVendido = pedidosNoCancelados.Sum(p => p.Total),
            UltimosPedidos = pedidos.Take(5).ToList(),
            ProductosConStockBajo = inventario
                .Where(i => i.StockBajo)
                .Take(5)
                .ToList(),
            AuditoriasRecientes = auditorias.Take(6).ToList()
        };

        return View(viewModel);
    }
}