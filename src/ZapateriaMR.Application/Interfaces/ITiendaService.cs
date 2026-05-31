using ZapateriaMR.Application.DTOs.Tienda;

namespace ZapateriaMR.Application.Interfaces;

public interface ITiendaService
{
    Task<IReadOnlyList<TiendaProductoListadoDto>> ObtenerProductosAsync(
        string? busqueda = null,
        int? categoriaId = null);

    Task<TiendaProductoDetalleDto?> ObtenerProductoPorIdAsync(int id);

    Task<IReadOnlyList<CategoriaTiendaDto>> ObtenerCategoriasAsync();
}