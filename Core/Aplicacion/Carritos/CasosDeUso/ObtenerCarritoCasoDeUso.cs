using VoltajeModa.Core.Aplicacion.Carritos.Interfaces;
using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Dominio.Carritos;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;

namespace VoltajeModa.Core.Aplicacion.Carritos.CasosDeUso;

public class ObtenerCarritoCasoDeUso
{
    private readonly IRepositorioCarritos _carritos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ObtenerCarritoCasoDeUso(IRepositorioCarritos carritos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _carritos = carritos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Carrito> EjecutarAsync(Titular titular, CancellationToken ct)
    {
        var carrito = titular.EsUsuario
            ? await _carritos.ObtenerPorUsuarioAsync(titular.UsuarioId!, ct)
            : await _carritos.ObtenerPorAnonimoAsync(titular.AnonimoId!, ct);

        if (carrito is not null)
        {
            return carrito;
        }

        carrito = Carrito.Crear(titular);
        await _carritos.AgregarAsync(carrito, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return carrito;
    }
}
