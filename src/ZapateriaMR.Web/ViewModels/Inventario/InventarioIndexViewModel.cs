using ZapateriaMR.Application.DTOs.Inventario;

namespace ZapateriaMR.Web.ViewModels.Inventario;

public class InventarioIndexViewModel
{
    public string? Busqueda { get; set; }

    public IReadOnlyList<InventarioListadoDto> Productos { get; set; } = [];
}