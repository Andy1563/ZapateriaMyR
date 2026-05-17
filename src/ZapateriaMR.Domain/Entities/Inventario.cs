using ZapateriaMR.Domain.Common;

namespace ZapateriaMR.Domain.Entities;

public class Inventario : BaseEntity
{
    public int ProductoId { get; set; }

    public Producto? Producto { get; set; }

    public int CantidadDisponible { get; set; }

    public int CantidadReservada { get; set; }

    public int StockMinimo { get; set; }

    public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow;
}