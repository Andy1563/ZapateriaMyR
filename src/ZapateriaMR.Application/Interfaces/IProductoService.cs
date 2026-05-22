using ZapateriaMR.Application.DTOs.Productos;

namespace ZapateriaMR.Application.Interfaces;

public interface IProductoService
{
    Task<IReadOnlyList<ProductoListadoDto>> ObtenerTodosAsync(string? busqueda = null);

    Task<ProductoDetalleDto?> ObtenerPorIdAsync(int id);

    Task<EditarProductoDto?> ObtenerParaEditarAsync(int id);

    Task<IReadOnlyList<CategoriaProductoSelectDto>> ObtenerCategoriasAsync();

    Task<int> CrearAsync(CrearProductoDto dto, string? usuarioId = null);

    Task<bool> EditarAsync(EditarProductoDto dto, string? usuarioId = null);

    Task<bool> DesactivarAsync(int id, string? usuarioId = null);
}