using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;

namespace VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;

public interface IRepositorioDirecciones
{
    Task<Direccion?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Direccion>> ListarPorTitularAsync(Titular titular, CancellationToken ct);
    Task AgregarAsync(Direccion direccion, CancellationToken ct);
    Task ActualizarAsync(Direccion direccion, CancellationToken ct);
    Task EliminarAsync(int id, CancellationToken ct);
}
