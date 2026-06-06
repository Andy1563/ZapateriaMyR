using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Application.DTOs.Reportes;

public class ReportePedidoPorEstadoDto
{
    public EstadoPedido Estado { get; set; }

    public int Cantidad { get; set; }

    public decimal Total { get; set; }
}