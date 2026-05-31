using ZapateriaMR.Application.Interfaces;

namespace ZapateriaMR.Web.Services.Carrito;

public class CarritoService : ICarritoService
{
    private const string CarritoSessionKey = "CarritoCompras";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITiendaService _tiendaService;

    public CarritoService(
        IHttpContextAccessor httpContextAccessor,
        ITiendaService tiendaService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tiendaService = tiendaService;
    }

    public IReadOnlyList<CarritoItem> ObtenerItems()
    {
        return ObtenerCarrito();
    }

    public async Task AgregarAsync(int productoId, int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
        }

        var producto = await _tiendaService.ObtenerProductoPorIdAsync(productoId);

        if (producto is null)
        {
            throw new InvalidOperationException("El producto no existe o no está disponible.");
        }

        var carrito = ObtenerCarrito();

        var itemExistente = carrito.FirstOrDefault(i => i.ProductoId == productoId);

        var cantidadActual = itemExistente?.Cantidad ?? 0;
        var nuevaCantidad = cantidadActual + cantidad;

        if (nuevaCantidad > producto.CantidadDisponible)
        {
            throw new InvalidOperationException("No hay suficiente stock disponible para agregar esa cantidad al carrito.");
        }

        if (itemExistente is null)
        {
            carrito.Add(new CarritoItem
            {
                ProductoId = producto.Id,
                CodigoSku = producto.CodigoSku,
                Nombre = producto.Nombre,
                ImagenUrl = producto.ImagenUrl,
                Marca = producto.Marca,
                Color = producto.Color,
                Talla = producto.Talla,
                PrecioUnitario = producto.PrecioVenta,
                Cantidad = cantidad,
                CantidadDisponible = producto.CantidadDisponible
            });
        }
        else
        {
            itemExistente.Cantidad = nuevaCantidad;
            itemExistente.CantidadDisponible = producto.CantidadDisponible;
        }

        GuardarCarrito(carrito);
    }

    public void ActualizarCantidad(int productoId, int cantidad)
    {
        var carrito = ObtenerCarrito();

        var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);

        if (item is null)
        {
            return;
        }

        if (cantidad <= 0)
        {
            carrito.Remove(item);
            GuardarCarrito(carrito);
            return;
        }

        if (cantidad > item.CantidadDisponible)
        {
            throw new InvalidOperationException("La cantidad indicada supera el stock disponible.");
        }

        item.Cantidad = cantidad;

        GuardarCarrito(carrito);
    }

    public void Eliminar(int productoId)
    {
        var carrito = ObtenerCarrito();

        var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);

        if (item is not null)
        {
            carrito.Remove(item);
            GuardarCarrito(carrito);
        }
    }

    public void Vaciar()
    {
        HttpContext.Session.Remove(CarritoSessionKey);
    }

    public decimal ObtenerTotal()
    {
        return ObtenerCarrito().Sum(i => i.Subtotal);
    }

    public int ObtenerCantidadTotal()
    {
        return ObtenerCarrito().Sum(i => i.Cantidad);
    }

    private List<CarritoItem> ObtenerCarrito()
    {
        return HttpContext.Session.GetObject<List<CarritoItem>>(CarritoSessionKey) ?? [];
    }

    private void GuardarCarrito(List<CarritoItem> carrito)
    {
        HttpContext.Session.SetObject(CarritoSessionKey, carrito);
    }

    private HttpContext HttpContext
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                throw new InvalidOperationException("No hay un contexto HTTP disponible.");
            }

            return httpContext;
        }
    }
}