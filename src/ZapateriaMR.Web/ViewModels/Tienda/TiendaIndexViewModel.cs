using Microsoft.AspNetCore.Mvc.Rendering;
using ZapateriaMR.Application.DTOs.Tienda;

namespace ZapateriaMR.Web.ViewModels.Tienda;

public class TiendaIndexViewModel
{
    public string? Busqueda { get; set; }

    public int? CategoriaId { get; set; }

    public IReadOnlyList<TiendaProductoListadoDto> Productos { get; set; } = [];

    public IEnumerable<SelectListItem> Categorias { get; set; } = [];
}