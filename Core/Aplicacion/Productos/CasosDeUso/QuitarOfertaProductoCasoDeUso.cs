using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class QuitarOfertaProductoCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public QuitarOfertaProductoCasoDeUso(IRepositorioProductos productos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task EjecutarAsync(int id, CancellationToken ct)
    {
        var producto = await _productos.ObtenerPorIdAsync(id, ct) ?? throw new ProductoNoEncontradoException(id);

        producto.QuitarOferta();
        await _productos.ActualizarAsync(producto, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
