using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Application.DTOs.Auditoria;

public class RegistrarAuditoriaDto
{
    public string? UsuarioId { get; set; }

    public string? NombreUsuario { get; set; }

    public TipoAccionAuditoria Accion { get; set; }

    public string EntidadAfectada { get; set; } = string.Empty;

    public string? RegistroId { get; set; }

    public string? Detalle { get; set; }

    public string? DireccionIp { get; set; }
}