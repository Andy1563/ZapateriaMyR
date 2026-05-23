namespace ZapateriaMR.Application.DTOs.Inventario;

public class InventarioListadoDto
{
    public int ProductoId { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string NombreProducto { get; set; } = string.Empty;

    public string? Marca { get; set; }

    public string? Color { get; set; }

    public string? Talla { get; set; }

    public string? ImagenUrl { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public int CantidadDisponible { get; set; }

    public int CantidadReservada { get; set; }

    public int StockMinimo { get; set; }

    public bool StockBajo => CantidadDisponible <= StockMinimo;

    public DateTime FechaUltimaActualizacion { get; set; }
}