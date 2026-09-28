using VoltajeModa.Core.Aplicacion.Pedidos.Dtos;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Pedidos;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

public class ListarPedidosCasoDeUso
{
    private readonly IRepositorioPedidos _pedidos;

    public ListarPedidosCasoDeUso(IRepositorioPedidos pedidos)
    {
        _pedidos = pedidos;
    }

    public Task<(IReadOnlyList<Pedido> Items, int Total)> EjecutarAsync(FiltroPedidos filtro, CancellationToken ct)
    {
        return _pedidos.ListarTodosAsync(filtro, ct);
    }
}
