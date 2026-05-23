using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZapateriaMR.Application.DTOs.Inventario;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Domain.Enums;
using ZapateriaMR.Web.ViewModels.Inventario;

namespace ZapateriaMR.Web.Controllers;

public class InventarioController : Controller
{
    private readonly IInventarioService _inventarioService;

    public InventarioController(IInventarioService inventarioService)
    {
        _inventarioService = inventarioService;
    }

    public async Task<IActionResult> Index(string? busqueda)
    {
        var inventario = await _inventarioService.ObtenerInventarioAsync(busqueda);

        var viewModel = new InventarioIndexViewModel
        {
            Busqueda = busqueda,
            Productos = inventario
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int productoId)
    {
        var inventario = await _inventarioService.ObtenerDetallePorProductoIdAsync(productoId);

        if (inventario is null)
        {
            return NotFound();
        }

        return View(inventario);
    }

    public async Task<IActionResult> RegistrarMovimiento(int productoId)
    {
        var inventario = await _inventarioService.ObtenerDetallePorProductoIdAsync(productoId);

        if (inventario is null)
        {
            return NotFound();
        }

        var viewModel = new RegistrarMovimientoInventarioViewModel
        {
            ProductoId = inventario.ProductoId,
            CodigoSku = inventario.CodigoSku,
            NombreProducto = inventario.NombreProducto,
            ImagenUrl = inventario.ImagenUrl,
            CantidadDisponibleActual = inventario.CantidadDisponible
        };

        CargarTiposMovimiento(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarMovimiento(RegistrarMovimientoInventarioViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            CargarTiposMovimiento(viewModel);
            return View(viewModel);
        }

        try
        {
            var dto = new RegistrarMovimientoInventarioDto
            {
                ProductoId = viewModel.ProductoId,
                TipoMovimiento = viewModel.TipoMovimiento,
                Cantidad = viewModel.Cantidad,
                Motivo = viewModel.Motivo
            };

            var registrado = await _inventarioService.RegistrarMovimientoAsync(dto, ObtenerUsuarioId());

            if (!registrado)
            {
                return NotFound();
            }

            TempData["Success"] = "Movimiento de inventario registrado correctamente.";

            return RedirectToAction(nameof(Details), new { productoId = viewModel.ProductoId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            CargarTiposMovimiento(viewModel);

            return View(viewModel);
        }
    }

    private void CargarTiposMovimiento(RegistrarMovimientoInventarioViewModel viewModel)
    {
        viewModel.TiposMovimiento =
        [
            new SelectListItem
            {
                Value = ((int)TipoMovimientoInventario.Entrada).ToString(),
                Text = "Entrada - Agregar stock"
            },
            new SelectListItem
            {
                Value = ((int)TipoMovimientoInventario.Salida).ToString(),
                Text = "Salida - Restar stock"
            },
            new SelectListItem
            {
                Value = ((int)TipoMovimientoInventario.Ajuste).ToString(),
                Text = "Ajuste - Definir stock final"
            }
        ];
    }

    private string? ObtenerUsuarioId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}