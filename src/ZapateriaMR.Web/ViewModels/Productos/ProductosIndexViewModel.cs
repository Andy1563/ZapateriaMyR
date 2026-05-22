using ZapateriaMR.Application.DTOs.Productos;

namespace ZapateriaMR.Web.ViewModels.Productos;

public class ProductosIndexViewModel
{
    public string? Busqueda { get; set; }

    public IReadOnlyList<ProductoListadoDto> Productos { get; set; } = [];
}