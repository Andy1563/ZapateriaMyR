using ZapateriaMR.Application.DTOs.Pedidos;

namespace ZapateriaMR.Application.Interfaces;

public interface IPedidoPdfService
{
    Task<byte[]> GenerarPedidoPdfAsync(PedidoDetalleDto pedido);
}