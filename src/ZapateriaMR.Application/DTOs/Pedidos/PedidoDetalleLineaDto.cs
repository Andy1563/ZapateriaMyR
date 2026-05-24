namespace ZapateriaMR.Application.DTOs.Pedidos;

public class PedidoDetalleLineaDto
{
    public int ProductoId { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string NombreProducto { get; set; } = string.Empty;

    public string? ImagenUrl { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }
}