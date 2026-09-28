using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class PonerProductoEnOfertaCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public PonerProductoEnOfertaCasoDeUso(IRepositorioProductos productos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Producto> EjecutarAsync(int id, decimal precioOferta, CancellationToken ct)
    {
        var producto = await _productos.ObtenerPorIdAsync(id, ct) ?? throw new ProductoNoEncontradoException(id);

        producto.PonerEnOferta(precioOferta);
        await _productos.ActualizarAsync(producto, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return producto;
    }
}
