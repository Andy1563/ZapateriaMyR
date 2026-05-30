using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ZapateriaMR.Web.ViewModels.Pedidos;

public class PedidoCreateViewModel
{
    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    [Display(Name = "Nombre del cliente")]
    public string NombreCliente { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    [Display(Name = "Correo del cliente")]
    public string? CorreoCliente { get; set; }

    [StringLength(30, ErrorMessage = "El teléfono no puede superar los 30 caracteres.")]
    [Display(Name = "Teléfono")]
    public string? TelefonoCliente { get; set; }

    [StringLength(300, ErrorMessage = "La dirección no puede superar los 300 caracteres.")]
    [Display(Name = "Dirección de entrega")]
    public string? DireccionEntrega { get; set; }

    [Display(Name = "Fecha estimada de entrega")]
    public DateTime? FechaEntregaEstimada { get; set; }

    [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
    public string? Observaciones { get; set; }

    public List<PedidoDetalleInputViewModel> Detalles { get; set; } =
    [
        new()
    ];

    public IEnumerable<SelectListItem> ProductosDisponibles { get; set; } = [];
}