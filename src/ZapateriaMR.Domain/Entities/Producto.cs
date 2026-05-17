using ZapateriaMR.Domain.Common;

namespace ZapateriaMR.Domain.Entities;

public class Producto : BaseEntity
{
    public string CodigoSku { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string? Marca { get; set; }

    public string? Color { get; set; }

    public string? Talla { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public int CategoriaProductoId { get; set; }

    public CategoriaProducto? CategoriaProducto { get; set; }

    public Inventario? Inventario { get; set; }

    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();

    public ICollection<MovimientoInventario> MovimientosInventario { get; set; } = new List<MovimientoInventario>();
}