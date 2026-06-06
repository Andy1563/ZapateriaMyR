using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZapateriaMR.Application.DTOs.Pedidos;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Infrastructure.Identity;
using ZapateriaMR.Web.Services.Carrito;
using ZapateriaMR.Web.ViewModels.Checkout;

namespace ZapateriaMR.Web.Controllers;

[Authorize(Roles = "Cliente")]
public class CheckoutController : Controller
{
    private readonly ICarritoService _carritoService;
    private readonly IPedidoService _pedidoService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CheckoutController(
        ICarritoService carritoService,
        IPedidoService pedidoService,
        UserManager<ApplicationUser> userManager)
    {
        _carritoService = carritoService;
        _pedidoService = pedidoService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var items = _carritoService.ObtenerItems();

        if (!items.Any())
        {
            TempData["Error"] = "El carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        var usuario = await _userManager.GetUserAsync(User);

        if (usuario is null)
        {
            return Challenge();
        }

        var viewModel = new CheckoutViewModel
        {
            NombreCliente = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
            CorreoCliente = usuario.Email,
            Items = items,
            Total = _carritoService.ObtenerTotal(),
            CantidadTotal = _carritoService.ObtenerCantidadTotal()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel viewModel)
    {
        var items = _carritoService.ObtenerItems();

        if (!items.Any())
        {
            TempData["Error"] = "El carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        viewModel.Items = items;
        viewModel.Total = _carritoService.ObtenerTotal();
        viewModel.CantidadTotal = _carritoService.ObtenerCantidadTotal();

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var usuario = await _userManager.GetUserAsync(User);

        if (usuario is null)
        {
            return Challenge();
        }

        try
        {
            var dto = new CrearPedidoDto
            {
                ClienteUsuarioId = usuario.Id,
                NombreCliente = viewModel.NombreCliente,
                CorreoCliente = viewModel.CorreoCliente,
                TelefonoCliente = viewModel.TelefonoCliente,
                DireccionEntrega = viewModel.DireccionEntrega,
                FechaEntregaEstimada = viewModel.FechaEntregaEstimada,
                Observaciones = viewModel.Observaciones,
                Detalles = items.Select(item => new CrearDetallePedidoDto
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad
                }).ToList()
            };

            var pedidoId = await _pedidoService.CrearAsync(dto, usuario.Id);

            _carritoService.Vaciar();

            return RedirectToAction(nameof(Confirmacion), new { id = pedidoId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            viewModel.Items = _carritoService.ObtenerItems();
            viewModel.Total = _carritoService.ObtenerTotal();
            viewModel.CantidadTotal = _carritoService.ObtenerCantidadTotal();

            return View(viewModel);
        }
    }

    public IActionResult Confirmacion(int id)
    {
        ViewBag.PedidoId = id;

        return View();
    }
}