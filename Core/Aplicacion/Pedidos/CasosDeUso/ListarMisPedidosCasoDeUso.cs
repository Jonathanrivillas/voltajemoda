using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

public class ListarMisPedidosCasoDeUso
{
    private readonly IRepositorioPedidos _pedidos;

    public ListarMisPedidosCasoDeUso(IRepositorioPedidos pedidos)
    {
        _pedidos = pedidos;
    }

    public Task<IReadOnlyList<Pedido>> EjecutarAsync(Titular titular, CancellationToken ct)
    {
        return _pedidos.ListarPorTitularAsync(titular, ct);
    }
}
