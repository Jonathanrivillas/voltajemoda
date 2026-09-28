using VoltajeModa.Core.Dominio.Productos;

namespace VoltajeModa.Core.Aplicacion.Productos.Interfaces;

public interface IRepositorioCategorias
{
    Task<Categoria?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Categoria>> ListarTodasAsync(CancellationToken ct);
    Task AgregarAsync(Categoria categoria, CancellationToken ct);
    Task ActualizarAsync(Categoria categoria, CancellationToken ct);
    Task EliminarAsync(int id, CancellationToken ct);
    Task<int> ContarProductosAsync(int categoriaId, CancellationToken ct);
}
