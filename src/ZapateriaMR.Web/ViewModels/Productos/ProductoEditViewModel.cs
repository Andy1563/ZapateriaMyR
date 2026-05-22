using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ZapateriaMR.Web.ViewModels.Productos;

public class ProductoEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código SKU es obligatorio.")]
    [StringLength(50, ErrorMessage = "El código SKU no puede superar los 50 caracteres.")]
    [Display(Name = "Código SKU")]
    public string CodigoSku { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    [StringLength(100, ErrorMessage = "La marca no puede superar los 100 caracteres.")]
    public string? Marca { get; set; }

    [StringLength(50, ErrorMessage = "El color no puede superar los 50 caracteres.")]
    public string? Color { get; set; }

    [StringLength(20, ErrorMessage = "La talla no puede superar los 20 caracteres.")]
    public string? Talla { get; set; }

    [Required(ErrorMessage = "El precio de compra es obligatorio.")]
    [Range(0, 999999999, ErrorMessage = "El precio de compra no puede ser negativo.")]
    [Display(Name = "Precio de compra")]
    public decimal PrecioCompra { get; set; }

    [Required(ErrorMessage = "El precio de venta es obligatorio.")]
    [Range(0, 999999999, ErrorMessage = "El precio de venta no puede ser negativo.")]
    [Display(Name = "Precio de venta")]
    public decimal PrecioVenta { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una categoría.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
    [Display(Name = "Categoría")]
    public int CategoriaProductoId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    [Display(Name = "Stock mínimo")]
    public int StockMinimo { get; set; }

    public bool Activo { get; set; }

    public string? ImagenActualUrl { get; set; }

    [Display(Name = "Nueva imagen")]
    public IFormFile? Imagen { get; set; }

    public IEnumerable<SelectListItem> Categorias { get; set; } = [];
}