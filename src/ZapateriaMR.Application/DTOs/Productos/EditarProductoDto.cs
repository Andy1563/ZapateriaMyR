namespace ZapateriaMR.Application.DTOs.Productos;

public class EditarProductoDto
{
    public int Id { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string? Marca { get; set; }

    public string? Color { get; set; }

    public string? Talla { get; set; }

    public string? ImagenUrl { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public int CategoriaProductoId { get; set; }

    public int StockMinimo { get; set; }

    public bool Activo { get; set; }
}