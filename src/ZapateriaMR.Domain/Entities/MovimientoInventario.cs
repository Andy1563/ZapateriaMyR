using ZapateriaMR.Domain.Common;
using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Domain.Entities;

public class MovimientoInventario : BaseEntity
{
    public int ProductoId { get; set; }

    public Producto? Producto { get; set; }

    public TipoMovimientoInventario TipoMovimiento { get; set; }

    public int Cantidad { get; set; }

    public int StockAnterior { get; set; }

    public int StockNuevo { get; set; }

    public string? Motivo { get; set; }

    public string? UsuarioId { get; set; }

    public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;
}