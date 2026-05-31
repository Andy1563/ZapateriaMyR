using ZapateriaMR.Web.Services.Carrito;

namespace ZapateriaMR.Web.ViewModels.Carrito;

public class CarritoIndexViewModel
{
    public IReadOnlyList<CarritoItem> Items { get; set; } = [];

    public decimal Total { get; set; }

    public int CantidadTotal { get; set; }
}