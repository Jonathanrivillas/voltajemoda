using VoltajeModa.Core.Dominio.Productos;

namespace VoltajeModa.Core.Aplicacion.Productos.Interfaces;

public interface IRepositorioCategorias
{
    Task<Categoria?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Categoria>> ListarTodasAsync(CancellationToken ct);
    Task AgregarAsync(Categoria categoria, CancellationToken ct);
}
