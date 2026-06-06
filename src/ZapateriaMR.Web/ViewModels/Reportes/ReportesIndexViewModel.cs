using ZapateriaMR.Application.DTOs.Reportes;

namespace ZapateriaMR.Web.ViewModels.Reportes;

public class ReportesIndexViewModel
{
    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public ReporteResumenDto Resumen { get; set; } = new();
}