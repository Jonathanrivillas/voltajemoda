using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;
using VoltajeModa.Core.Dominio.Pedidos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

public class ObtenerPedidoPorIdCasoDeUso
{
    private readonly IRepositorioPedidos _pedidos;

    public ObtenerPedidoPorIdCasoDeUso(IRepositorioPedidos pedidos)
    {
        _pedidos = pedidos;
    }

    // Si el pedido no pertenece al titular solicitante y no es administrador, se responde igual
    // que "no encontrado" (404) en vez de 403, para no filtrar la existencia de pedidos ajenos.
    public async Task<Pedido> EjecutarAsync(int id, Titular titular, bool esAdministrador, CancellationToken ct)
    {
        var pedido = await _pedidos.ObtenerPorIdAsync(id, ct) ?? throw new PedidoNoEncontradoException(id);

        if (!esAdministrador && !pedido.Titular.Coincide(titular.UsuarioId, titular.AnonimoId))
        {
            throw new PedidoNoEncontradoException(id);
        }

        return pedido;
    }
}
