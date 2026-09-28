namespace VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;

// Puerto angosto hacia el bounded context de Inventario. Implementado por
// AjustadorStockInventarioAdapter, que NO hace commit por su cuenta: el commit único de todo el
// checkout lo dispara CrearPedidoDesdeCarritoCasoDeUso al final, para que el descuento de stock,
// la creación del pedido y el vaciado del carrito queden en una sola transacción atómica.
public interface IAjustadorStock
{
    Task<ResultadoAjusteStock> DescontarPorVentaAsync(int productoVarianteId, int cantidad, string motivo, string? usuarioId, CancellationToken ct);
}

public record ResultadoAjusteStock(bool Exito, string? Error);
