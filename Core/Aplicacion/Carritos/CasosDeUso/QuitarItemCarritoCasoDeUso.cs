using VoltajeModa.Core.Aplicacion.Carritos.Interfaces;
using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;

namespace VoltajeModa.Core.Aplicacion.Carritos.CasosDeUso;

public class QuitarItemCarritoCasoDeUso
{
    private readonly ObtenerCarritoCasoDeUso _obtenerCarrito;
    private readonly IRepositorioCarritos _carritos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public QuitarItemCarritoCasoDeUso(ObtenerCarritoCasoDeUso obtenerCarrito, IRepositorioCarritos carritos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _obtenerCarrito = obtenerCarrito;
        _carritos = carritos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task EjecutarAsync(Titular titular, int productoVarianteId, CancellationToken ct)
    {
        var carrito = await _obtenerCarrito.EjecutarAsync(titular, ct);
        carrito.QuitarItem(productoVarianteId);

        await _carritos.ActualizarAsync(carrito, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
