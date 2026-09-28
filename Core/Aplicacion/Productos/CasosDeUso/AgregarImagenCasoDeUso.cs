using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class AgregarImagenCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public AgregarImagenCasoDeUso(IRepositorioProductos productos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<ProductoImagen> EjecutarAsync(int productoId, string url, int orden, CancellationToken ct)
    {
        var producto = await _productos.ObtenerPorIdAsync(productoId, ct) ?? throw new ProductoNoEncontradoException(productoId);

        producto.AgregarImagen(url, orden);
        await _productos.ActualizarAsync(producto, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        var productoActualizado = await _productos.ObtenerPorIdAsync(productoId, ct) ?? throw new ProductoNoEncontradoException(productoId);
        return productoActualizado.Imagenes.Last(i => i.Url == url.Trim() && i.Orden == orden);
    }
}
