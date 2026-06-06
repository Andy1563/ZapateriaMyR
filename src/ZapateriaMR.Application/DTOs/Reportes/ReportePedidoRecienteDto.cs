using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Application.DTOs.Reportes;

public class ReportePedidoRecienteDto
{
    public int PedidoId { get; set; }

    public string NumeroPedido { get; set; } = string.Empty;

    public string NombreCliente { get; set; } = string.Empty;

    public EstadoPedido Estado { get; set; }

    public decimal Total { get; set; }

    public DateTime FechaPedido { get; set; }
}