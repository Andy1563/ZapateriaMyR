using ZapateriaMR.Application.DTOs.Auditoria;

namespace ZapateriaMR.Web.ViewModels.Auditoria;

public class AuditoriaIndexViewModel
{
    public string? Busqueda { get; set; }

    public IReadOnlyList<AuditoriaListadoDto> Auditorias { get; set; } = [];
}