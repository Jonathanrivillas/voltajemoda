using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

public class ListarMisDireccionesCasoDeUso
{
    private readonly IRepositorioDirecciones _direcciones;

    public ListarMisDireccionesCasoDeUso(IRepositorioDirecciones direcciones)
    {
        _direcciones = direcciones;
    }

    public Task<IReadOnlyList<Direccion>> EjecutarAsync(Titular titular, CancellationToken ct)
    {
        return _direcciones.ListarPorTitularAsync(titular, ct);
    }
}
