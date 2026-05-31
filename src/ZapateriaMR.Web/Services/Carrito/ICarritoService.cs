namespace ZapateriaMR.Web.Services.Carrito;

public interface ICarritoService
{
    IReadOnlyList<CarritoItem> ObtenerItems();

    Task AgregarAsync(int productoId, int cantidad);

    void ActualizarCantidad(int productoId, int cantidad);

    void Eliminar(int productoId);

    void Vaciar();

    decimal ObtenerTotal();

    int ObtenerCantidadTotal();
}