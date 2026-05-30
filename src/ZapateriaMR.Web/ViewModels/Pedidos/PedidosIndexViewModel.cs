using ZapateriaMR.Application.DTOs.Pedidos;

namespace ZapateriaMR.Web.ViewModels.Pedidos;

public class PedidosIndexViewModel
{
    public string? Busqueda { get; set; }

    public IReadOnlyList<PedidoListadoDto> Pedidos { get; set; } = [];
}