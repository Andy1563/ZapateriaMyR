using System.ComponentModel.DataAnnotations;

namespace ZapateriaMR.Web.ViewModels.Pedidos;

public class PedidoDetalleInputViewModel
{
    [Display(Name = "Producto")]
    public int ProductoId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
    public int Cantidad { get; set; }
}