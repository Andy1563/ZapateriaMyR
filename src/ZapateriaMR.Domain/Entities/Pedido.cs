using ZapateriaMR.Domain.Common;
using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Domain.Entities;

public class Pedido : BaseEntity
{
    public string NumeroPedido { get; set; } = string.Empty;

    public string? ClienteUsuarioId { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public string? CorreoCliente { get; set; }

    public string? TelefonoCliente { get; set; }

    public string? DireccionEntrega { get; set; }

    public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

    public decimal Subtotal { get; set; }

    public decimal Total { get; set; }

    public DateTime FechaPedido { get; set; } = DateTime.UtcNow;

    public DateTime? FechaEntregaEstimada { get; set; }

    public string? Observaciones { get; set; }

    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
}