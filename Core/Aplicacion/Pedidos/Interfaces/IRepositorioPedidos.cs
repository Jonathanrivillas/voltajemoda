using VoltajeModa.Core.Aplicacion.Pedidos.Dtos;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;

namespace VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;

public interface IRepositorioPedidos
{
    Task<Pedido?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Pedido>> ListarPorTitularAsync(Titular titular, CancellationToken ct);
    Task<(IReadOnlyList<Pedido> Items, int Total)> ListarTodosAsync(FiltroPedidos filtro, CancellationToken ct);
    Task AgregarAsync(Pedido pedido, CancellationToken ct);
    Task ActualizarAsync(Pedido pedido, CancellationToken ct);
}
