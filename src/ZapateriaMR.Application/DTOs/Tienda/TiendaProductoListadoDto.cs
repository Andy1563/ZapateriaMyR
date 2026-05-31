namespace ZapateriaMR.Application.DTOs.Tienda;

public class TiendaProductoListadoDto
{
    public int Id { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Marca { get; set; }

    public string? Color { get; set; }

    public string? Talla { get; set; }

    public string? ImagenUrl { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public decimal PrecioVenta { get; set; }

    public int CantidadDisponible { get; set; }
}