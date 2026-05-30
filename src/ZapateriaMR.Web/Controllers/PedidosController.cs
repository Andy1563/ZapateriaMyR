using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZapateriaMR.Application.DTOs.Pedidos;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Enums;
using ZapateriaMR.Web.ViewModels.Pedidos;

namespace ZapateriaMR.Web.Controllers;

public class PedidosController : Controller
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    public async Task<IActionResult> Index(string? busqueda)
    {
        var pedidos = await _pedidoService.ObtenerPedidosAsync(busqueda);

        var viewModel = new PedidosIndexViewModel
        {
            Busqueda = busqueda,
            Pedidos = pedidos
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var pedido = await _pedidoService.ObtenerPorIdAsync(id);

        if (pedido is null)
        {
            return NotFound();
        }

        ViewBag.Estados = ObtenerEstadosPedido(pedido.Estado);

        return View(pedido);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new PedidoCreateViewModel();

        await CargarProductosDisponiblesAsync(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PedidoCreateViewModel viewModel)
    {
        var detallesValidos = viewModel.Detalles
            .Where(d => d.ProductoId > 0 && d.Cantidad > 0)
            .ToList();

        if (!detallesValidos.Any())
        {
            ModelState.AddModelError(string.Empty, "Debe agregar al menos un producto con cantidad mayor a cero.");
        }

        if (!ModelState.IsValid)
        {
            await CargarProductosDisponiblesAsync(viewModel);
            return View(viewModel);
        }

        try
        {
            var dto = new CrearPedidoDto
            {
                NombreCliente = viewModel.NombreCliente,
                CorreoCliente = viewModel.CorreoCliente,
                TelefonoCliente = viewModel.TelefonoCliente,
                DireccionEntrega = viewModel.DireccionEntrega,
                FechaEntregaEstimada = viewModel.FechaEntregaEstimada,
                Observaciones = viewModel.Observaciones,
                Detalles = detallesValidos.Select(d => new CrearDetallePedidoDto
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad
                }).ToList()
            };

            var pedidoId = await _pedidoService.CrearAsync(dto, ObtenerUsuarioId());

            TempData["Success"] = "Pedido creado correctamente. El inventario fue actualizado.";

            return RedirectToAction(nameof(Details), new { id = pedidoId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            await CargarProductosDisponiblesAsync(viewModel);

            return View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int pedidoId, EstadoPedido nuevoEstado)
    {
        try
        {
            var actualizado = await _pedidoService.CambiarEstadoAsync(
                pedidoId,
                nuevoEstado,
                ObtenerUsuarioId());

            if (!actualizado)
            {
                return NotFound();
            }

            TempData["Success"] = "Estado del pedido actualizado correctamente.";

            return RedirectToAction(nameof(Details), new { id = pedidoId });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(nameof(Details), new { id = pedidoId });
        }
    }

    private async Task CargarProductosDisponiblesAsync(PedidoCreateViewModel viewModel)
    {
        var productos = await _pedidoService.ObtenerProductosDisponiblesAsync();

        viewModel.ProductosDisponibles = productos.Select(p => new SelectListItem
        {
            Value = p.ProductoId.ToString(),
            Text = $"{p.NombreCompleto} - Disponible: {p.CantidadDisponible} - ₡{p.PrecioVenta:N2}"
        });
    }

    private static IEnumerable<SelectListItem> ObtenerEstadosPedido(EstadoPedido estadoActual)
    {
        return Enum.GetValues<EstadoPedido>()
            .Select(estado => new SelectListItem
            {
                Value = ((int)estado).ToString(),
                Text = estado.ToString(),
                Selected = estado == estadoActual
            });
    }

    private string? ObtenerUsuarioId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}