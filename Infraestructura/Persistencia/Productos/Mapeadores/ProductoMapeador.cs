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

        SincronizarColeccion(
            entidad.Variantes, v => v.Id, producto.Variantes.Select(v => v.Id).Where(id => id != 0).ToHashSet(),
            producto.Variantes.Where(v => v.Id == 0),
            v => new EfModels.ProductoVariante { Talla = v.Talla, Color = v.Color, Stock = v.Stock });

        SincronizarColeccion(
            entidad.ImagenesAdicionales, i => i.Id, producto.Imagenes.Select(i => i.Id).Where(id => id != 0).ToHashSet(),
            producto.Imagenes.Where(i => i.Id == 0),
            i => new EfModels.ProductoImagen { Url = i.Url, Orden = i.Orden });
    }

    // Sincroniza una colección hija completa contra el estado actual del agregado de dominio:
    // elimina (de la colección trackeada) las filas cuyo id ya no está entre los ids vigentes del
    // dominio, y agrega como filas nuevas las que el dominio tiene con Id == 0 (recién creadas).
    private static void SincronizarColeccion<TDominio, TEf>(
        ICollection<TEf> entidadesEf, Func<TEf, int> idDeEf, HashSet<int> idsVigentesEnDominio,
        IEnumerable<TDominio> nuevasEnDominio, Func<TDominio, TEf> crear)
    {
        foreach (var efExistente in entidadesEf.Where(e => !idsVigentesEnDominio.Contains(idDeEf(e))).ToList())
        {
            entidadesEf.Remove(efExistente);
        }

        foreach (var nueva in nuevasEnDominio)
        {
            entidadesEf.Add(crear(nueva));
        }
    }
}
