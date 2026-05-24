using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Application.DTOs.Pedidos;

public class CambiarEstadoPedidoDto
{
    public int PedidoId { get; set; }

    public EstadoPedido NuevoEstado { get; set; }
}