using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Application.DTOs.Pedidos;

public class PedidoDetalleDto
{
    public int Id { get; set; }

    public string NumeroPedido { get; set; } = string.Empty;

    public string NombreCliente { get; set; } = string.Empty;

    public string? CorreoCliente { get; set; }

    public string? TelefonoCliente { get; set; }

    public string? DireccionEntrega { get; set; }

    public EstadoPedido Estado { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Total { get; set; }

    public DateTime FechaPedido { get; set; }

    public DateTime? FechaEntregaEstimada { get; set; }

    public string? Observaciones { get; set; }

    public IReadOnlyList<PedidoDetalleLineaDto> Detalles { get; set; } = [];
}