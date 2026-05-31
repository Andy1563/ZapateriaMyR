using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Web.ViewModels.Tienda;

namespace ZapateriaMR.Web.Controllers;

[AllowAnonymous]
public class TiendaController : Controller
{
    private readonly ITiendaService _tiendaService;

    public TiendaController(ITiendaService tiendaService)
    {
        _tiendaService = tiendaService;
    }

    public async Task<IActionResult> Index(string? busqueda, int? categoriaId)
    {
        var productos = await _tiendaService.ObtenerProductosAsync(busqueda, categoriaId);
        var categorias = await _tiendaService.ObtenerCategoriasAsync();

        var viewModel = new TiendaIndexViewModel
        {
            Busqueda = busqueda,
            CategoriaId = categoriaId,
            Productos = productos,
            Categorias = categorias.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nombre,
                Selected = categoriaId.HasValue && categoriaId.Value == c.Id
            })
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var producto = await _tiendaService.ObtenerProductoPorIdAsync(id);

        if (producto is null)
        {
            return NotFound();
        }

        return View(producto);
    }
}