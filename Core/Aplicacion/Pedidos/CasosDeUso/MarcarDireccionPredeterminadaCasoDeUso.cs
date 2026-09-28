using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;
using VoltajeModa.Core.Dominio.Pedidos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

public class MarcarDireccionPredeterminadaCasoDeUso
{
    private readonly IRepositorioDirecciones _direcciones;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public MarcarDireccionPredeterminadaCasoDeUso(IRepositorioDirecciones direcciones, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _direcciones = direcciones;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task EjecutarAsync(Titular titular, int direccionId, CancellationToken ct)
    {
        var direccion = await _direcciones.ObtenerPorIdAsync(direccionId, ct) ?? throw new DireccionNoEncontradaException(direccionId);
        if (!direccion.Titular.Coincide(titular.UsuarioId, titular.AnonimoId))
        {
            throw new DireccionNoEncontradaException(direccionId);
        }

        var direccionesDelTitular = await _direcciones.ListarPorTitularAsync(titular, ct);
        foreach (var otra in direccionesDelTitular.Where(d => d.Id != direccionId && d.EsPredeterminada))
        {
            otra.QuitarPredeterminada();
            await _direcciones.ActualizarAsync(otra, ct);
        }

        direccion.MarcarComoPredeterminada();
        await _direcciones.ActualizarAsync(direccion, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
