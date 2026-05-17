using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZapateriaMR.Domain.Entities;
using ZapateriaMR.Infrastructure.Identity;

namespace ZapateriaMR.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CategoriaProducto> CategoriasProducto { get; set; }

    public DbSet<Producto> Productos { get; set; }

    public DbSet<Inventario> Inventarios { get; set; }

    public DbSet<MovimientoInventario> MovimientosInventario { get; set; }

    public DbSet<Pedido> Pedidos { get; set; }

    public DbSet<DetallePedido> DetallesPedido { get; set; }

    public DbSet<Auditoria> Auditorias { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CategoriaProducto>(entity =>
        {
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(300);
        });

        builder.Entity<Producto>(entity =>
        {
            entity.Property(e => e.CodigoSku)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(e => e.CodigoSku)
                .IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500);

            entity.Property(e => e.Marca)
                .HasMaxLength(100);

            entity.Property(e => e.Color)
                .HasMaxLength(50);

            entity.Property(e => e.Talla)
                .HasMaxLength(20);

            entity.Property(e => e.PrecioCompra)
                .HasPrecision(18, 2);

            entity.Property(e => e.PrecioVenta)
                .HasPrecision(18, 2);

            entity.HasOne(e => e.CategoriaProducto)
                .WithMany(e => e.Productos)
                .HasForeignKey(e => e.CategoriaProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Inventario>(entity =>
        {
            entity.HasOne(e => e.Producto)
                .WithOne(e => e.Inventario)
                .HasForeignKey<Inventario>(e => e.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<MovimientoInventario>(entity =>
        {
            entity.Property(e => e.Motivo)
                .HasMaxLength(300);

            entity.Property(e => e.UsuarioId)
                .HasMaxLength(450);

            entity.HasOne(e => e.Producto)
                .WithMany(e => e.MovimientosInventario)
                .HasForeignKey(e => e.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Pedido>(entity =>
        {
            entity.Property(e => e.NumeroPedido)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(e => e.NumeroPedido)
                .IsUnique();

            entity.Property(e => e.ClienteUsuarioId)
                .HasMaxLength(450);

            entity.Property(e => e.NombreCliente)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(e => e.CorreoCliente)
                .HasMaxLength(150);

            entity.Property(e => e.TelefonoCliente)
                .HasMaxLength(30);

            entity.Property(e => e.DireccionEntrega)
                .HasMaxLength(300);

            entity.Property(e => e.Observaciones)
                .HasMaxLength(500);

            entity.Property(e => e.Subtotal)
                .HasPrecision(18, 2);

            entity.Property(e => e.Total)
                .HasPrecision(18, 2);
        });

        builder.Entity<DetallePedido>(entity =>
        {
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(18, 2);

            entity.Property(e => e.Subtotal)
                .HasPrecision(18, 2);

            entity.HasOne(e => e.Pedido)
                .WithMany(e => e.Detalles)
                .HasForeignKey(e => e.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Producto)
                .WithMany(e => e.DetallesPedido)
                .HasForeignKey(e => e.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Auditoria>(entity =>
        {
            entity.Property(e => e.UsuarioId)
                .HasMaxLength(450);

            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(150);

            entity.Property(e => e.EntidadAfectada)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.RegistroId)
                .HasMaxLength(100);

            entity.Property(e => e.Detalle)
                .HasMaxLength(1000);

            entity.Property(e => e.DireccionIp)
                .HasMaxLength(50);
        });
    }
}