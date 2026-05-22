namespace ZapateriaMR.Application.DTOs.Productos;

public class ProductoDetalleDto
{
    public int Id { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string? Marca { get; set; }

    public string? Color { get; set; }

    public string? Talla { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public int CantidadDisponible { get; set; }

    public int CantidadReservada { get; set; }

    public int StockMinimo { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }
}