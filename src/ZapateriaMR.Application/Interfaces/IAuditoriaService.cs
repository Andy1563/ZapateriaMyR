using ZapateriaMR.Application.DTOs.Auditoria;

namespace ZapateriaMR.Application.Interfaces;

public interface IAuditoriaService
{
    Task RegistrarAsync(RegistrarAuditoriaDto dto);

    Task<IReadOnlyList<AuditoriaListadoDto>> ObtenerAuditoriasAsync(string? busqueda = null);
}