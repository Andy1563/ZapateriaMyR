namespace ZapateriaMR.Application.DTOs.Reportes;

public class ReporteStockBajoDto
{
    public int ProductoId { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string NombreProducto { get; set; } = string.Empty;

    public string? ImagenUrl { get; set; }

    public int CantidadDisponible { get; set; }

    public int StockMinimo { get; set; }
}