using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class SubirImagenPrincipalCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IAlmacenamientoImagenes _almacenamiento;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public SubirImagenPrincipalCasoDeUso(IRepositorioProductos productos, IAlmacenamientoImagenes almacenamiento, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _almacenamiento = almacenamiento;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Producto> EjecutarAsync(int productoId, Stream contenido, string extension, long tamanoBytes, CancellationToken ct)
    {
        var producto = await _productos.ObtenerPorIdAsync(productoId, ct) ?? throw new ProductoNoEncontradoException(productoId);

        PoliticaImagen.ValidarArchivo(extension, tamanoBytes);
        var url = await _almacenamiento.GuardarAsync(contenido, extension, ct);

        producto.ActualizarImagenPrincipal(url);
        await _productos.ActualizarAsync(producto, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return producto;
    }
}
