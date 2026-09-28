using VoltajeModa.Core.Aplicacion.Inventario.Dtos;
using VoltajeModa.Core.Dominio.Inventario;

namespace VoltajeModa.Core.Aplicacion.Inventario.Interfaces;

public interface IRepositorioMovimientosStock
{
    Task AgregarAsync(MovimientoStock movimiento, CancellationToken ct);
    Task<(IReadOnlyList<MovimientoStock> Items, int Total)> ListarAsync(FiltroHistorial filtro, CancellationToken ct);
}
