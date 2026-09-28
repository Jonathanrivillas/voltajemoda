using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Pedidos;
using VoltajeModa.Core.Dominio.Pedidos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

public class CambiarEstadoPedidoCasoDeUso
{
    private readonly IRepositorioPedidos _pedidos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public CambiarEstadoPedidoCasoDeUso(IRepositorioPedidos pedidos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _pedidos = pedidos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Pedido> EjecutarAsync(int id, EstadoPedido nuevoEstado, CancellationToken ct)
    {
        var pedido = await _pedidos.ObtenerPorIdAsync(id, ct) ?? throw new PedidoNoEncontradoException(id);

        pedido.CambiarEstado(nuevoEstado);
        await _pedidos.ActualizarAsync(pedido, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return pedido;
    }
}
