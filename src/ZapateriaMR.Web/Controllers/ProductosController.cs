using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZapateriaMR.Application.DTOs.Productos;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Web.Services;
using ZapateriaMR.Web.ViewModels.Productos;

namespace ZapateriaMR.Web.Controllers;

public class ProductosController : Controller
{
    private readonly IProductoService _productoService;
    private readonly IImageStorageService _imageStorageService;

    public ProductosController(
        IProductoService productoService,
        IImageStorageService imageStorageService)
    {
        _productoService = productoService;
        _imageStorageService = imageStorageService;
    }

    public async Task<IActionResult> Index(string? busqueda)
    {
        var productos = await _productoService.ObtenerTodosAsync(busqueda);

        var viewModel = new ProductosIndexViewModel
        {
            Busqueda = busqueda,
            Productos = productos
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);

        if (producto is null)
        {
            return NotFound();
        }

        return View(producto);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new ProductoCreateViewModel();

        await CargarCategoriasAsync(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductoCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(viewModel);
            return View(viewModel);
        }

        try
        {
            var imagenUrl = await _imageStorageService.SaveProductImageAsync(viewModel.Imagen);

            var dto = new CrearProductoDto
            {
                CodigoSku = viewModel.CodigoSku,
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                Marca = viewModel.Marca,
                Color = viewModel.Color,
                Talla = viewModel.Talla,
                PrecioCompra = viewModel.PrecioCompra,
                PrecioVenta = viewModel.PrecioVenta,
                CategoriaProductoId = viewModel.CategoriaProductoId,
                CantidadInicial = viewModel.CantidadInicial,
                StockMinimo = viewModel.StockMinimo,
                ImagenUrl = imagenUrl
            };

            var productoId = await _productoService.CrearAsync(dto, ObtenerUsuarioId());

            TempData["Success"] = "Producto creado correctamente.";

            return RedirectToAction(nameof(Details), new { id = productoId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            await CargarCategoriasAsync(viewModel);

            return View(viewModel);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var producto = await _productoService.ObtenerParaEditarAsync(id);

        if (producto is null)
        {
            return NotFound();
        }

        var viewModel = new ProductoEditViewModel
        {
            Id = producto.Id,
            CodigoSku = producto.CodigoSku,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Marca = producto.Marca,
            Color = producto.Color,
            Talla = producto.Talla,
            PrecioCompra = producto.PrecioCompra,
            PrecioVenta = producto.PrecioVenta,
            CategoriaProductoId = producto.CategoriaProductoId,
            StockMinimo = producto.StockMinimo,
            Activo = producto.Activo,
            ImagenActualUrl = producto.ImagenUrl
        };

        await CargarCategoriasAsync(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductoEditViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(viewModel);
            return View(viewModel);
        }

        try
        {
            var nuevaImagenUrl = await _imageStorageService.SaveProductImageAsync(viewModel.Imagen);

            var dto = new EditarProductoDto
            {
                Id = viewModel.Id,
                CodigoSku = viewModel.CodigoSku,
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                Marca = viewModel.Marca,
                Color = viewModel.Color,
                Talla = viewModel.Talla,
                PrecioCompra = viewModel.PrecioCompra,
                PrecioVenta = viewModel.PrecioVenta,
                CategoriaProductoId = viewModel.CategoriaProductoId,
                StockMinimo = viewModel.StockMinimo,
                Activo = viewModel.Activo,
                ImagenUrl = nuevaImagenUrl
            };

            var actualizado = await _productoService.EditarAsync(dto, ObtenerUsuarioId());

            if (!actualizado)
            {
                return NotFound();
            }

            TempData["Success"] = "Producto actualizado correctamente.";

            return RedirectToAction(nameof(Details), new { id = viewModel.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            await CargarCategoriasAsync(viewModel);

            return View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        var desactivado = await _productoService.DesactivarAsync(id, ObtenerUsuarioId());

        if (!desactivado)
        {
            return NotFound();
        }

        TempData["Success"] = "Producto desactivado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCategoriasAsync(ProductoCreateViewModel viewModel)
    {
        var categorias = await _productoService.ObtenerCategoriasAsync();

        viewModel.Categorias = categorias.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Nombre
        });
    }

    private async Task CargarCategoriasAsync(ProductoEditViewModel viewModel)
    {
        var categorias = await _productoService.ObtenerCategoriasAsync();

        viewModel.Categorias = categorias.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Nombre
        });
    }

    private string? ObtenerUsuarioId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}