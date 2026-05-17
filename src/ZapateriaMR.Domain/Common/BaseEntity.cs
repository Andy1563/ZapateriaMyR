namespace ZapateriaMR.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public bool Activo { get; set; } = true;

    public bool EstaEliminado { get; set; } = false;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacionId { get; set; }

    public string? UsuarioModificacionId { get; set; }
}