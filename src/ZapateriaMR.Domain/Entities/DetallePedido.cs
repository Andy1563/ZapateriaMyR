using ZapateriaMR.Domain.Common;

namespace ZapateriaMR.Domain.Entities;

public class DetallePedido : BaseEntity
{
    public int PedidoId { get; set; }

    public Pedido? Pedido { get; set; }

    public int ProductoId { get; set; }

    public Producto? Producto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }
}