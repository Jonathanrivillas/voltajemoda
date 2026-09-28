namespace VoltajeModa.Core.Aplicacion.Productos.Interfaces;

// Puerto angosto hacia Carrito/Pedidos/Inventario: antes de eliminar una variante hay que saber si
// está referenciada en algún carrito, algún pedido activo (no cancelado) o algún movimiento de
// stock, igual que valida hoy el panel de administración.
public interface IVerificadorUsoVariante
{
    Task<bool> EstaEnUsoAsync(int productoVarianteId, CancellationToken ct);
}
