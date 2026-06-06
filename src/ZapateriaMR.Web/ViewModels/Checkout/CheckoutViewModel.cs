using System.ComponentModel.DataAnnotations;
using ZapateriaMR.Web.Services.Carrito;

namespace ZapateriaMR.Web.ViewModels.Checkout;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    [Display(Name = "Nombre del cliente")]
    public string NombreCliente { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string? CorreoCliente { get; set; }

    [StringLength(30, ErrorMessage = "El teléfono no puede superar los 30 caracteres.")]
    [Display(Name = "Teléfono")]
    public string? TelefonoCliente { get; set; }

    [Required(ErrorMessage = "La dirección de entrega es obligatoria.")]
    [StringLength(300, ErrorMessage = "La dirección no puede superar los 300 caracteres.")]
    [Display(Name = "Dirección de entrega")]
    public string DireccionEntrega { get; set; } = string.Empty;

    [Display(Name = "Fecha estimada de entrega")]
    public DateTime? FechaEntregaEstimada { get; set; }

    [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
    public string? Observaciones { get; set; }

    public IReadOnlyList<CarritoItem> Items { get; set; } = [];

    public decimal Total { get; set; }

    public int CantidadTotal { get; set; }
}