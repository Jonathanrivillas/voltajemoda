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

        var superiores = new Categoria { Nombre = "Superiores" };
        var inferiores = new Categoria { Nombre = "Inferiores" };
        var conjuntos = new Categoria { Nombre = "Conjuntos" };
        var vestidos = new Categoria { Nombre = "Vestidos" };
        var enterizos = new Categoria { Nombre = "Enterizos" };

        context.Categorias.AddRange(superiores, inferiores, conjuntos, vestidos, enterizos);

        context.Productos.AddRange(
            new Producto
            {
                Nombre = "Camiseta Voltaje Classic",
                Descripcion = "Camiseta de algodón 100%, corte regular.",
                Precio = 25000,
                ImagenUrl = "https://picsum.photos/seed/camiseta-voltaje/500/500",
                Categoria = superiores,
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
                Categoria = inferiores,
                Variantes = new List<ProductoVariante>
                {
                    new() { Talla = "30", Color = "Verde", Stock = 6 },
                    new() { Talla = "32", Color = "Verde", Stock = 9 },
                    new() { Talla = "34", Color = "Beige", Stock = 5 },
                }
            },
            new Producto
            {
                Nombre = "Conjunto Voltaje Duo",
                Descripcion = "Conjunto de top y pantalón a juego, tela liviana de lino.",
                Precio = 145000,
                EnOferta = true,
                PrecioOferta = 115000,
                ImagenUrl = "https://picsum.photos/seed/conjunto-voltaje/500/500",
                Categoria = conjuntos,
                Variantes = new List<ProductoVariante>
                {
                    new() { Talla = "S", Color = "Beige", Stock = 5 },
                    new() { Talla = "M", Color = "Beige", Stock = 7 },
                }
            },
            new Producto
            {
                Nombre = "Vestido Voltaje Sunset",
                Descripcion = "Vestido midi con tirantes, ideal para clima cálido.",
                Precio = 98000,
                ImagenUrl = "https://picsum.photos/seed/vestido-voltaje/500/500",
                Categoria = vestidos,
                Variantes = new List<ProductoVariante>
                {
                    new() { Talla = "S", Color = "Terracota", Stock = 6 },
                    new() { Talla = "M", Color = "Terracota", Stock = 8 },
                }
            },
            new Producto
            {
                Nombre = "Enterizo Voltaje Breeze",
                Descripcion = "Enterizo fresco de manga corta, corte amplio.",
                Precio = 110000,
                ImagenUrl = "https://picsum.photos/seed/enterizo-voltaje/500/500",
                Categoria = enterizos,
                Variantes = new List<ProductoVariante>
                {
                    new() { Talla = "M", Color = "Blanco", Stock = 4 },
                    new() { Talla = "L", Color = "Blanco", Stock = 6 },
                }
            }
        );

        context.SaveChanges();
    }
}
