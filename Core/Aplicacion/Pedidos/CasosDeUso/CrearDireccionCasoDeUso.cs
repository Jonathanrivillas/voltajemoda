using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

public class CrearDireccionCasoDeUso
{
    private readonly IRepositorioDirecciones _direcciones;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public CrearDireccionCasoDeUso(IRepositorioDirecciones direcciones, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _direcciones = direcciones;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Direccion> EjecutarAsync(
        Titular titular, string? etiqueta, string? nombreDestinatario, string? telefono,
        string calle, string ciudad, string codigoPostal, bool esPredeterminada, CancellationToken ct)
    {
        var direccion = Direccion.Crear(titular, etiqueta, nombreDestinatario, telefono, calle, ciudad, codigoPostal, esPredeterminada);
        await _direcciones.AgregarAsync(direccion, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return direccion;
    }
}
