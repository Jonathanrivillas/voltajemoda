using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoltajeModa.Models;

namespace VoltajeModa.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<ProductoVariante> ProductoVariantes => Set<ProductoVariante>();
    public DbSet<ProductoImagen> ProductoImagenes => Set<ProductoImagen>();
    public DbSet<Direccion> Direcciones => Set<Direccion>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<CarritoItem> CarritoItems => Set<CarritoItem>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoItem> PedidoItems => Set<PedidoItem>();
    public DbSet<MovimientoStock> MovimientosStock => Set<MovimientoStock>();
    public DbSet<BannerSlide> BannerSlides => Set<BannerSlide>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MovimientoStock>()
            .HasOne(m => m.ProductoVariante)
            .WithMany()
            .HasForeignKey(m => m.ProductoVarianteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MovimientoStock>()
            .Property(m => m.Motivo)
            .HasMaxLength(200);

        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Direccion)
            .WithMany()
            .HasForeignKey(p => p.DireccionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Direccion>()
            .Property(d => d.Etiqueta)
            .HasMaxLength(30)
            .HasDefaultValue("Casa");
    }
}
