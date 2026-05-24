using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Application.DTOs.Pedidos;

public class PedidoListadoDto
{
    public int Id { get; set; }

    public string NumeroPedido { get; set; } = string.Empty;

    public string NombreCliente { get; set; } = string.Empty;

    public EstadoPedido Estado { get; set; }

    public decimal Total { get; set; }

    public int CantidadProductos { get; set; }

    public DateTime FechaPedido { get; set; }

    public DateTime? FechaEntregaEstimada { get; set; }
}