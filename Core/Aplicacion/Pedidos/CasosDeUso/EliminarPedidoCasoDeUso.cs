using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Pedidos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

public class EliminarPedidoCasoDeUso
{
    private readonly IRepositorioPedidos _pedidos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public EliminarPedidoCasoDeUso(IRepositorioPedidos pedidos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _pedidos = pedidos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task EjecutarAsync(int id, CancellationToken ct)
    {
        var pedido = await _pedidos.ObtenerPorIdAsync(id, ct) ?? throw new PedidoNoEncontradoException(id);

        pedido.AsegurarEsEliminable();
        await _pedidos.EliminarAsync(id, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
