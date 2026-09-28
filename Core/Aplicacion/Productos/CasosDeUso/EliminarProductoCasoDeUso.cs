using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class EliminarProductoCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public EliminarProductoCasoDeUso(IRepositorioProductos productos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task EjecutarAsync(int id, CancellationToken ct)
    {
        _ = await _productos.ObtenerPorIdAsync(id, ct) ?? throw new ProductoNoEncontradoException(id);

        await _productos.EliminarAsync(id, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
