using ZapateriaMR.Domain.Common;

namespace ZapateriaMR.Domain.Entities;

public class CategoriaProducto : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}