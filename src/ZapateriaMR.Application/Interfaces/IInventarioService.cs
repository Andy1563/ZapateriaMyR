using ZapateriaMR.Application.DTOs.Inventario;

namespace ZapateriaMR.Application.Interfaces;

public interface IInventarioService
{
    Task<IReadOnlyList<InventarioListadoDto>> ObtenerInventarioAsync(string? busqueda = null);

    Task<InventarioDetalleDto?> ObtenerDetallePorProductoIdAsync(int productoId);

    Task<bool> RegistrarMovimientoAsync(RegistrarMovimientoInventarioDto dto, string? usuarioId = null);
}