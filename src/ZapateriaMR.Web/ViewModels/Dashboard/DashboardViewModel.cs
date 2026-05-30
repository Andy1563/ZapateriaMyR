using ZapateriaMR.Application.DTOs.Auditoria;
using ZapateriaMR.Application.DTOs.Inventario;
using ZapateriaMR.Application.DTOs.Pedidos;

namespace ZapateriaMR.Web.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalProductos { get; set; }

    public int ProductosStockBajo { get; set; }

    public int PedidosPendientes { get; set; }

    public int PedidosCancelados { get; set; }

    public decimal TotalVendido { get; set; }

    public IReadOnlyList<PedidoListadoDto> UltimosPedidos { get; set; } = [];

    public IReadOnlyList<InventarioListadoDto> ProductosConStockBajo { get; set; } = [];

    public IReadOnlyList<AuditoriaListadoDto> AuditoriasRecientes { get; set; } = [];
}