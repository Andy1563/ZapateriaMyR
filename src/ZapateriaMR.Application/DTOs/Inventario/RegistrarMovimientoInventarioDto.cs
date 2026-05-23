using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Application.DTOs.Inventario;

public class RegistrarMovimientoInventarioDto
{
    public int ProductoId { get; set; }

    public TipoMovimientoInventario TipoMovimiento { get; set; }

    public int Cantidad { get; set; }

    public string? Motivo { get; set; }
}