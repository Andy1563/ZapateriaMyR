using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Web.ViewModels.Inventario;

public class RegistrarMovimientoInventarioViewModel
{
    public int ProductoId { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string NombreProducto { get; set; } = string.Empty;

    public string? ImagenUrl { get; set; }

    public int CantidadDisponibleActual { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el tipo de movimiento.")]
    [Display(Name = "Tipo de movimiento")]
    public TipoMovimientoInventario TipoMovimiento { get; set; } = TipoMovimientoInventario.Entrada;

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
    public int Cantidad { get; set; }

    [StringLength(300, ErrorMessage = "El motivo no puede superar los 300 caracteres.")]
    public string? Motivo { get; set; }

    public IEnumerable<SelectListItem> TiposMovimiento { get; set; } = [];
}