using VoltajeModa.Models;

namespace VoltajeModa.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Categorias.Any())
        {
            return;
        }

        var camisetas = new Categoria { Nombre = "Camisetas" };
        var pantalones = new Categoria { Nombre = "Pantalones" };
        var chaquetas = new Categoria { Nombre = "Chaquetas" };

        context.Categorias.AddRange(camisetas, pantalones, chaquetas);

        context.Productos.AddRange(
            new Producto
            {
                Nombre = "Camiseta Voltaje Classic",
                Descripcion = "Camiseta de algodón 100%, corte regular.",
                Precio = 25000,
                ImagenUrl = "https://picsum.photos/seed/camiseta-voltaje/500/500",
                Categoria = camisetas,
                Variantes = new List<ProductoVariante>
                {
                    new() { Talla = "S", Color = "Negro", Stock = 10 },
                    new() { Talla = "M", Color = "Negro", Stock = 15 },
                    new() { Talla = "L", Color = "Negro", Stock = 8 },
                    new() { Talla = "M", Color = "Blanco", Stock = 12 },
                }
            },
            new Producto
            {
                Nombre = "Pantalón Cargo Voltaje",
                Descripcion = "Pantalón cargo resistente, múltiples bolsillos.",
                Precio = 68000,
                ImagenUrl = "https://picsum.photos/seed/pantalon-voltaje/500/500",
                Categoria = pantalones,
                Variantes = new List<ProductoVariante>
                {
                    new() { Talla = "30", Color = "Verde", Stock = 6 },
                    new() { Talla = "32", Color = "Verde", Stock = 9 },
                    new() { Talla = "34", Color = "Beige", Stock = 5 },
                }
            },
            new Producto
            {
                Nombre = "Chaqueta Voltaje Urban",
                Descripcion = "Chaqueta impermeable ligera, ideal para entretiempo.",
                Precio = 120000,
                EnOferta = true,
                PrecioOferta = 95000,
                ImagenUrl = "https://picsum.photos/seed/chaqueta-voltaje/500/500",
                Categoria = chaquetas,
                Variantes = new List<ProductoVariante>
                {
                    new() { Talla = "M", Color = "Azul", Stock = 4 },
                    new() { Talla = "L", Color = "Azul", Stock = 7 },
                    new() { Talla = "L", Color = "Negro", Stock = 3 },
                }
            }
        );

        context.SaveChanges();
    }
}
