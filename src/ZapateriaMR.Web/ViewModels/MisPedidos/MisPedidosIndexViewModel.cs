using ZapateriaMR.Application.DTOs.Pedidos;

namespace ZapateriaMR.Web.ViewModels.MisPedidos;

public class MisPedidosIndexViewModel
{
    public string? Busqueda { get; set; }

    public IReadOnlyList<PedidoListadoDto> Pedidos { get; set; } = [];
}