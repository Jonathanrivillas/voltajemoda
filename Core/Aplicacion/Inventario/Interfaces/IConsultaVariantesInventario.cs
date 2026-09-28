using VoltajeModa.Core.Aplicacion.Inventario.Dtos;

namespace VoltajeModa.Core.Aplicacion.Inventario.Interfaces;

public interface IConsultaVariantesInventario
{
    Task<int?> ObtenerStockActualAsync(int productoVarianteId, CancellationToken ct);
    Task ActualizarStockAsync(int productoVarianteId, int nuevoStock, CancellationToken ct);
    Task<IReadOnlyList<VarianteInventarioLectura>> ListarAsync(FiltroVariantes filtro, CancellationToken ct);
}
