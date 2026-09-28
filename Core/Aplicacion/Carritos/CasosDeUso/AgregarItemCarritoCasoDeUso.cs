using VoltajeModa.Core.Aplicacion.Carritos.Interfaces;
using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Dominio.Carritos;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Inventario.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Carritos.CasosDeUso;

public class AgregarItemCarritoCasoDeUso
{
    private readonly ObtenerCarritoCasoDeUso _obtenerCarrito;
    private readonly IConsultaVariantesCarrito _variantes;
    private readonly IRepositorioCarritos _carritos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public AgregarItemCarritoCasoDeUso(
        ObtenerCarritoCasoDeUso obtenerCarrito, IConsultaVariantesCarrito variantes,
        IRepositorioCarritos carritos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _obtenerCarrito = obtenerCarrito;
        _variantes = variantes;
        _carritos = carritos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Carrito> EjecutarAsync(Titular titular, int productoVarianteId, int cantidad, CancellationToken ct)
    {
        if (!await _variantes.ExisteAsync(productoVarianteId, ct))
        {
            throw new VarianteNoEncontradaException(productoVarianteId);
        }

        var carrito = await _obtenerCarrito.EjecutarAsync(titular, ct);
        carrito.AgregarItem(productoVarianteId, cantidad);

        await _carritos.ActualizarAsync(carrito, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return carrito;
    }
}
