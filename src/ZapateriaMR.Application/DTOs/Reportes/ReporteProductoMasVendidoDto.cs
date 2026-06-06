namespace ZapateriaMR.Application.DTOs.Reportes;

public class ReporteProductoMasVendidoDto
{
    public int ProductoId { get; set; }

    public string CodigoSku { get; set; } = string.Empty;

    public string NombreProducto { get; set; } = string.Empty;

    public int UnidadesVendidas { get; set; }

    public decimal TotalVendido { get; set; }
}