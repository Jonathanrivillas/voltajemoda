using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class CrearCategoriaCasoDeUso
{
    private readonly IRepositorioCategorias _categorias;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public CrearCategoriaCasoDeUso(IRepositorioCategorias categorias, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _categorias = categorias;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Categoria> EjecutarAsync(string nombre, CancellationToken ct)
    {
        var categoria = Categoria.Crear(nombre);
        await _categorias.AgregarAsync(categoria, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return categoria;
    }
}
