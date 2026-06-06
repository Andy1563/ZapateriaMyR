using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Web.ViewModels.Reportes;

namespace ZapateriaMR.Web.Controllers;

[Authorize(Roles = "Administrador,UsuarioDueño")]
public class ReportesController : Controller
{
    private readonly IReporteService _reporteService;

    public ReportesController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var resumen = await _reporteService.ObtenerResumenAsync(fechaInicio, fechaFin);

        var viewModel = new ReportesIndexViewModel
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Resumen = resumen
        };

        return View(viewModel);
    }
}