using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapateriaMR.Web.Services.Carrito;
using ZapateriaMR.Web.ViewModels.Carrito;

namespace ZapateriaMR.Web.Controllers;

[AllowAnonymous]
public class CarritoController : Controller
{
    private readonly ICarritoService _carritoService;

    public CarritoController(ICarritoService carritoService)
    {
        _carritoService = carritoService;
    }

    public IActionResult Index()
    {
        var viewModel = new CarritoIndexViewModel
        {
            Items = _carritoService.ObtenerItems(),
            Total = _carritoService.ObtenerTotal(),
            CantidadTotal = _carritoService.ObtenerCantidadTotal()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(int productoId, int cantidad = 1, string? returnUrl = null)
    {
        try
        {
            await _carritoService.AgregarAsync(productoId, cantidad);

            TempData["Success"] = "Producto agregado al carrito correctamente.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Actualizar(int productoId, int cantidad)
    {
        try
        {
            _carritoService.ActualizarCantidad(productoId, cantidad);

            TempData["Success"] = "Carrito actualizado correctamente.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int productoId)
    {
        _carritoService.Eliminar(productoId);

        TempData["Success"] = "Producto eliminado del carrito.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Vaciar()
    {
        _carritoService.Vaciar();

        TempData["Success"] = "Carrito vaciado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}