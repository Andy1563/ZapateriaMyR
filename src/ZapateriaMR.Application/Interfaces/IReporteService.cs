using ZapateriaMR.Application.DTOs.Reportes;

namespace ZapateriaMR.Application.Interfaces;

public interface IReporteService
{
    Task<ReporteResumenDto> ObtenerResumenAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
}