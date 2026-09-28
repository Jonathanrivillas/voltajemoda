using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class EliminarCategoriaCasoDeUso
{
    private readonly IRepositorioCategorias _categorias;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public EliminarCategoriaCasoDeUso(IRepositorioCategorias categorias, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _categorias = categorias;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task EjecutarAsync(int id, CancellationToken ct)
    {
        var categoria = await _categorias.ObtenerPorIdAsync(id, ct) ?? throw new CategoriaNoEncontradaException(id);

        var cantidadProductos = await _categorias.ContarProductosAsync(id, ct);
        if (cantidadProductos > 0)
        {
            throw new CategoriaConProductosException(categoria.Nombre, cantidadProductos);
        }

        await _categorias.EliminarAsync(id, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
