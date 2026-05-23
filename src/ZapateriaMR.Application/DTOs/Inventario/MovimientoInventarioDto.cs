using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Application.DTOs.Inventario;

public class MovimientoInventarioDto
{
    public int Id { get; set; }

    public TipoMovimientoInventario TipoMovimiento { get; set; }

    public int Cantidad { get; set; }

    public int StockAnterior { get; set; }

    public int StockNuevo { get; set; }

    public string? Motivo { get; set; }

    public string? UsuarioId { get; set; }

    public DateTime FechaMovimiento { get; set; }
}