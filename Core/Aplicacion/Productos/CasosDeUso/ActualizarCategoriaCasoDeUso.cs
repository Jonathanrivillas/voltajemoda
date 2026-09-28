using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class ActualizarCategoriaCasoDeUso
{
    private readonly IRepositorioCategorias _categorias;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ActualizarCategoriaCasoDeUso(IRepositorioCategorias categorias, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _categorias = categorias;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Categoria> EjecutarAsync(int id, string nombre, CancellationToken ct)
    {
        var categoria = await _categorias.ObtenerPorIdAsync(id, ct) ?? throw new CategoriaNoEncontradaException(id);

        categoria.ActualizarNombre(nombre);
        await _categorias.ActualizarAsync(categoria, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return categoria;
    }
}
