using VoltajeModa.Core.Aplicacion.Inventario.Dtos;
using VoltajeModa.Core.Aplicacion.Inventario.Interfaces;
using VoltajeModa.Core.Dominio.Inventario;

namespace VoltajeModa.Core.Aplicacion.Inventario.CasosDeUso;

public class ListarHistorialMovimientosCasoDeUso
{
    private readonly IRepositorioMovimientosStock _movimientos;

    public ListarHistorialMovimientosCasoDeUso(IRepositorioMovimientosStock movimientos)
    {
        _movimientos = movimientos;
    }

    public Task<(IReadOnlyList<MovimientoStock> Items, int Total)> EjecutarAsync(FiltroHistorial filtro, CancellationToken ct)
    {
        return _movimientos.ListarAsync(filtro, ct);
    }
}
