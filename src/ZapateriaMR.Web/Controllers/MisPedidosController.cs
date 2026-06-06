using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Web.ViewModels.MisPedidos;


namespace ZapateriaMR.Web.Controllers;

[Authorize(Roles = "Cliente")]
public class MisPedidosController : Controller
{
    private readonly IPedidoService _pedidoService;
    private readonly IPedidoPdfService _pedidoPdfService;

    public MisPedidosController(
    IPedidoService pedidoService,
    IPedidoPdfService pedidoPdfService)
    {
        _pedidoService = pedidoService;
        _pedidoPdfService = pedidoPdfService;
    }

    public async Task<IActionResult> Index(string? busqueda)
    {
        var usuarioId = ObtenerUsuarioId();

        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return Challenge();
        }

        var pedidos = await _pedidoService.ObtenerPedidosPorClienteAsync(usuarioId, busqueda);

        var viewModel = new MisPedidosIndexViewModel
        {
            Busqueda = busqueda,
            Pedidos = pedidos
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var usuarioId = ObtenerUsuarioId();

        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return Challenge();
        }

        var pedido = await _pedidoService.ObtenerPedidoClientePorIdAsync(id, usuarioId);

        if (pedido is null)
        {
            return NotFound();
        }

        return View(pedido);
    }

    private string? ObtenerUsuarioId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public async Task<IActionResult> DescargarPdf(int id)
    {
        var usuarioId = ObtenerUsuarioId();

        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return Challenge();
        }

        var pedido = await _pedidoService.ObtenerPedidoClientePorIdAsync(id, usuarioId);

        if (pedido is null)
        {
            return NotFound();
        }

        var pdf = await _pedidoPdfService.GenerarPedidoPdfAsync(pedido);

        var fileName = $"pedido-{pedido.NumeroPedido}.pdf";

        return File(pdf, "application/pdf", fileName);
    }
}