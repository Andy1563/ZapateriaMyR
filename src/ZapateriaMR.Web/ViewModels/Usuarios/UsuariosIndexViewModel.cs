namespace ZapateriaMR.Web.ViewModels.Usuarios;

public class UsuariosIndexViewModel
{
    public string? Busqueda { get; set; }

    public IReadOnlyList<UsuarioListadoViewModel> Usuarios { get; set; } = [];
}

public class UsuarioListadoViewModel
{
    public string Id { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string Roles { get; set; } = string.Empty;
}