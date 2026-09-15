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
    public DbSet<Direccion> Direcciones => Set<Direccion>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<CarritoItem> CarritoItems => Set<CarritoItem>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoItem> PedidoItems => Set<PedidoItem>();
    public DbSet<MovimientoStock> MovimientosStock => Set<MovimientoStock>();

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
    }
}
