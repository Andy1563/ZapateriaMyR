namespace ZapateriaMR.Web.Services.Carrito;

public class CarritoItem
{
    public int ProductoId { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? ImagenUrl { get; set; }

    public string? Marca { get; set; }

    public string? Color { get; set; }

    public string? Talla { get; set; }

    public decimal PrecioUnitario { get; set; }

    public int Cantidad { get; set; }

    public int CantidadDisponible { get; set; }

    public decimal Subtotal => PrecioUnitario * Cantidad;
}