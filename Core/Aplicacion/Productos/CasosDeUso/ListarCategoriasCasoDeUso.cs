using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class ListarCategoriasCasoDeUso
{
    private readonly IRepositorioCategorias _categorias;

    public ListarCategoriasCasoDeUso(IRepositorioCategorias categorias)
    {
        _categorias = categorias;
    }

    public Task<IReadOnlyList<Categoria>> EjecutarAsync(CancellationToken ct)
    {
        return _categorias.ListarTodasAsync(ct);
    }
}
