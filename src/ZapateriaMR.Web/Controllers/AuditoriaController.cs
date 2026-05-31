using Microsoft.AspNetCore.Mvc;
using ZapateriaMR.Application.Interfaces;
using ZapateriaMR.Web.ViewModels.Auditoria;
using Microsoft.AspNetCore.Authorization;

namespace ZapateriaMR.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class AuditoriaController : Controller
{
    private readonly IAuditoriaService _auditoriaService;

    public AuditoriaController(IAuditoriaService auditoriaService)
    {
        _auditoriaService = auditoriaService;
    }

    public async Task<IActionResult> Index(string? busqueda)
    {
        var auditorias = await _auditoriaService.ObtenerAuditoriasAsync(busqueda);

        var viewModel = new AuditoriaIndexViewModel
        {
            Busqueda = busqueda,
            Auditorias = auditorias
        };

        return View(viewModel);
    }
}