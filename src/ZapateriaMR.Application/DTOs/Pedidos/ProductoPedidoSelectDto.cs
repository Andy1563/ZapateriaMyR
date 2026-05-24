namespace ZapateriaMR.Application.DTOs.Pedidos;

public class ProductoPedidoSelectDto
{
    public int ProductoId { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Marca { get; set; }

    public string? Color { get; set; }

    public string? Talla { get; set; }

    public string? ImagenUrl { get; set; }

    public decimal PrecioVenta { get; set; }

    public int CantidadDisponible { get; set; }

    public string NombreCompleto
    {
        get
        {
            var detalles = new List<string>();

            if (!string.IsNullOrWhiteSpace(Marca))
            {
                detalles.Add(Marca);
            }

            if (!string.IsNullOrWhiteSpace(Color))
            {
                detalles.Add(Color);
            }

            if (!string.IsNullOrWhiteSpace(Talla))
            {
                detalles.Add($"Talla {Talla}");
            }

            var detalleTexto = detalles.Count > 0
                ? $" ({string.Join(" · ", detalles)})"
                : string.Empty;

            return $"{CodigoSku} - {Nombre}{detalleTexto}";
        }
    }
}