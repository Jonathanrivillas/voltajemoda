using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class SubirImagenAdicionalCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IAlmacenamientoImagenes _almacenamiento;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public SubirImagenAdicionalCasoDeUso(IRepositorioProductos productos, IAlmacenamientoImagenes almacenamiento, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _almacenamiento = almacenamiento;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<ProductoImagen> EjecutarAsync(int productoId, Stream contenido, string extension, long tamanoBytes, int orden, CancellationToken ct)
    {
        var producto = await _productos.ObtenerPorIdAsync(productoId, ct) ?? throw new ProductoNoEncontradoException(productoId);

        PoliticaImagen.ValidarArchivo(extension, tamanoBytes);
        var url = await _almacenamiento.GuardarAsync(contenido, extension, ct);

        producto.AgregarImagen(url, orden);
        await _productos.ActualizarAsync(producto, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        var productoActualizado = await _productos.ObtenerPorIdAsync(productoId, ct) ?? throw new ProductoNoEncontradoException(productoId);
        return productoActualizado.Imagenes.Last(i => i.Url == url);
    }
}
