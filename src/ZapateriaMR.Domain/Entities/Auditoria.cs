using ZapateriaMR.Domain.Enums;

namespace ZapateriaMR.Domain.Entities;

public class Auditoria
{
    public int Id { get; set; }

    public string? UsuarioId { get; set; }

    public string? NombreUsuario { get; set; }

    public TipoAccionAuditoria Accion { get; set; }

    public string EntidadAfectada { get; set; } = string.Empty;

    public string? RegistroId { get; set; }

    public string? Detalle { get; set; }

    public string? DireccionIp { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}