using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Inventario.Interfaces;
using VoltajeModa.Core.Dominio.Inventario;
using VoltajeModa.Core.Dominio.Inventario.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Inventario.CasosDeUso;

// Punto único de escritura de Stock: tanto InventarioController (movimientos manuales) como el
// checkout de Pedidos deben pasar, directa o indirectamente, por esta misma regla de dominio.
public class RegistrarMovimientoStockCasoDeUso
{
    private readonly IConsultaVariantesInventario _variantes;
    private readonly IRepositorioMovimientosStock _movimientos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public RegistrarMovimientoStockCasoDeUso(
        IConsultaVariantesInventario variantes, IRepositorioMovimientosStock movimientos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _variantes = variantes;
        _movimientos = movimientos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<MovimientoStock> EjecutarAsync(
        int productoVarianteId, TipoMovimiento tipo, int cantidad, string motivo, string? usuarioId, CancellationToken ct)
    {
        var stockActual = await _variantes.ObtenerStockActualAsync(productoVarianteId, ct)
            ?? throw new VarianteNoEncontradaException(productoVarianteId);

        var (movimiento, nuevoStock) = MovimientoStock.Registrar(productoVarianteId, tipo, cantidad, motivo, usuarioId, stockActual);

        await _movimientos.AgregarAsync(movimiento, ct);
        await _variantes.ActualizarStockAsync(productoVarianteId, nuevoStock, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return movimiento;
    }
}
