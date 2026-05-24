using ZapateriaMR.Application.DTOs.Pedidos;
using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Application.Interfaces;

public interface IPedidoService
{
    Task<IReadOnlyList<PedidoListadoDto>> ObtenerPedidosAsync(string? busqueda = null);

    Task<PedidoDetalleDto?> ObtenerPorIdAsync(int id);

    Task<IReadOnlyList<ProductoPedidoSelectDto>> ObtenerProductosDisponiblesAsync();

    Task<int> CrearAsync(CrearPedidoDto dto, string? usuarioId = null);

    Task<bool> CambiarEstadoAsync(int pedidoId, EstadoPedido nuevoEstado, string? usuarioId = null);
}