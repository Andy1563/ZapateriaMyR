using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Web.ViewModels.Pedidos;

public class CambiarEstadoPedidoViewModel
{
    public int PedidoId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    [Display(Name = "Nuevo estado")]
    public EstadoPedido NuevoEstado { get; set; }

    public IEnumerable<SelectListItem> Estados { get; set; } = [];
}