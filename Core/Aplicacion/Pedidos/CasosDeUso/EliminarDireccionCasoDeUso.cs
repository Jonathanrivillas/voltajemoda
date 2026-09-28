using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

public class EliminarDireccionCasoDeUso
{
    private readonly IRepositorioDirecciones _direcciones;
    private readonly IRepositorioPedidos _pedidos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public EliminarDireccionCasoDeUso(IRepositorioDirecciones direcciones, IRepositorioPedidos pedidos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _direcciones = direcciones;
        _pedidos = pedidos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task EjecutarAsync(Titular titular, int direccionId, CancellationToken ct)
    {
        var direccion = await _direcciones.ObtenerPorIdAsync(direccionId, ct) ?? throw new DireccionNoEncontradaException(direccionId);
        if (!direccion.Titular.Coincide(titular.UsuarioId, titular.AnonimoId))
        {
            throw new DireccionNoEncontradaException(direccionId);
        }

        if (await _pedidos.ExisteAlgunoConDireccionAsync(direccionId, ct))
        {
            throw new DireccionEnUsoException();
        }

        await _direcciones.EliminarAsync(direccionId, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
