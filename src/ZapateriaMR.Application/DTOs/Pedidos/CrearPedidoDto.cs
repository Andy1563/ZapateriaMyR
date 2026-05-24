namespace ZapateriaMR.Application.DTOs.Pedidos;

public class CrearPedidoDto
{
    public string NombreCliente { get; set; } = string.Empty;

    public string? CorreoCliente { get; set; }

    public string? TelefonoCliente { get; set; }

    public string? DireccionEntrega { get; set; }

    public DateTime? FechaEntregaEstimada { get; set; }

    public string? Observaciones { get; set; }

    public List<CrearDetallePedidoDto> Detalles { get; set; } = [];
}