using VoltajeModa.Core.Dominio.Productos;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Productos.Mapeadores;

public static class ProductoMapeador
{
    public static Producto ADominio(EfModels.Producto entidad)
    {
        var variantes = entidad.Variantes.Select(v => new ProductoVariante(v.Id, v.Talla, v.Color, v.Stock));
        var imagenes = entidad.ImagenesAdicionales.Select(i => new ProductoImagen(i.Id, i.Url, i.Orden));

        return Producto.Reconstituir(
            entidad.Id, entidad.Nombre, entidad.Descripcion, entidad.Precio, entidad.ImagenUrl,
            entidad.EnOferta, entidad.PrecioOferta, entidad.Destacado, entidad.EsNuevo, entidad.CategoriaId,
            variantes, imagenes);
    }

    public static EfModels.Producto AEntidadNueva(Producto producto)
    {
        return new EfModels.Producto
        {
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            ImagenUrl = producto.ImagenUrl,
            EnOferta = producto.EnOferta,
            PrecioOferta = producto.PrecioOferta,
            Destacado = producto.Destacado,
            EsNuevo = producto.EsNuevo,
            CategoriaId = producto.CategoriaId,
            Variantes = producto.Variantes
                .Select(v => new EfModels.ProductoVariante { Talla = v.Talla, Color = v.Color, Stock = v.Stock })
                .ToList(),
            ImagenesAdicionales = producto.Imagenes
                .Select(i => new EfModels.ProductoImagen { Url = i.Url, Orden = i.Orden })
                .ToList()
        };
    }

    // Copia los campos escalares sobre la entidad EF ya trackeada (no la reemplaza) y solo agrega
    // las líneas nuevas de Variantes/Imágenes (Id == 0 del lado del dominio); las existentes se
    // dejan intactas para no disparar un reemplazo de colección que EF interprete como
    // borrar-todo-e-insertar-todo, lo cual rompería MovimientoStock (DeleteBehavior.Restrict).
    public static void VolcarEnEntidad(Producto producto, EfModels.Producto entidad)
    {
        entidad.Nombre = producto.Nombre;
        entidad.Descripcion = producto.Descripcion;
        entidad.Precio = producto.Precio;
        entidad.ImagenUrl = producto.ImagenUrl;
        entidad.EnOferta = producto.EnOferta;
        entidad.PrecioOferta = producto.PrecioOferta;
        entidad.Destacado = producto.Destacado;
        entidad.EsNuevo = producto.EsNuevo;
        entidad.CategoriaId = producto.CategoriaId;

        foreach (var variante in producto.Variantes.Where(v => v.Id == 0))
        {
            entidad.Variantes.Add(new EfModels.ProductoVariante { Talla = variante.Talla, Color = variante.Color, Stock = variante.Stock });
        }

        foreach (var imagen in producto.Imagenes.Where(i => i.Id == 0))
        {
            entidad.ImagenesAdicionales.Add(new EfModels.ProductoImagen { Url = imagen.Url, Orden = imagen.Orden });
        }
    }
}
