namespace ZapateriaMR.Application.DTOs.Reportes;

public class ReporteResumenDto
{
    public int TotalProductosActivos { get; set; }

    public int TotalUnidadesInventario { get; set; }

    public int ProductosStockBajo { get; set; }

    public int PedidosPendientes { get; set; }

    public int PedidosEntregados { get; set; }

    public int PedidosCancelados { get; set; }

    public decimal TotalVendido { get; set; }

    public IReadOnlyList<ReportePedidoPorEstadoDto> PedidosPorEstado { get; set; } = [];

    public IReadOnlyList<ReporteProductoMasVendidoDto> ProductosMasVendidos { get; set; } = [];

    public IReadOnlyList<ReporteStockBajoDto> ProductosConStockBajo { get; set; } = [];

    public IReadOnlyList<ReportePedidoRecienteDto> PedidosRecientes { get; set; } = [];
}