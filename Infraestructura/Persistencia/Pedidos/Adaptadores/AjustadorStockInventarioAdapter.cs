using VoltajeModa.Core.Aplicacion.Inventario.Interfaces;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun;
using VoltajeModa.Core.Dominio.Inventario;

namespace VoltajeModa.Infraestructura.Persistencia.Pedidos.Adaptadores;

// Implementa el puerto angosto IAjustadorStock (Pedidos) apoyándose directamente en los puertos de
// Inventario, SIN pasar por RegistrarMovimientoStockCasoDeUso: ese caso de uso hace su propio
// commit (IUnidadDeTrabajo.GuardarCambiosAsync) para el endpoint administrativo de movimientos
// manuales, lo cual rompería la atomicidad del checkout si se reutilizara aquí. Este adaptador solo
// registra el movimiento y ajusta el stock en el ApplicationDbContext trackeado; el commit único lo
// dispara CrearPedidoDesdeCarritoCasoDeUso al final, junto con el pedido y el vaciado del carrito.
public class AjustadorStockInventarioAdapter : IAjustadorStock
{
    private readonly IConsultaVariantesInventario _variantes;
    private readonly IRepositorioMovimientosStock _movimientos;

    public AjustadorStockInventarioAdapter(IConsultaVariantesInventario variantes, IRepositorioMovimientosStock movimientos)
    {
        _variantes = variantes;
        _movimientos = movimientos;
    }

    public async Task<ResultadoAjusteStock> DescontarPorVentaAsync(
        int productoVarianteId, int cantidad, string motivo, string? usuarioId, CancellationToken ct)
    {
        var stockActual = await _variantes.ObtenerStockActualAsync(productoVarianteId, ct);
        if (stockActual is null)
        {
            return new ResultadoAjusteStock(false, $"La variante {productoVarianteId} no existe.");
        }

        MovimientoStock movimiento;
        int nuevoStock;
        try
        {
            (movimiento, nuevoStock) = MovimientoStock.Registrar(productoVarianteId, TipoMovimiento.Salida, cantidad, motivo, usuarioId, stockActual.Value);
        }
        catch (ExcepcionDominio ex)
        {
            return new ResultadoAjusteStock(false, ex.Message);
        }

        await _movimientos.AgregarAsync(movimiento, ct);
        await _variantes.ActualizarStockAsync(productoVarianteId, nuevoStock, ct);

        return new ResultadoAjusteStock(true, null);
    }
}
