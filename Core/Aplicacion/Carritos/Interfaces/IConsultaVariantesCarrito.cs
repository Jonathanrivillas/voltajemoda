namespace VoltajeModa.Core.Aplicacion.Carritos.Interfaces;

// Puerto angosto hacia el bounded context de Productos: el carrito solo necesita saber si la
// variante existe, no el agregado completo.
public interface IConsultaVariantesCarrito
{
    Task<bool> ExisteAsync(int productoVarianteId, CancellationToken ct);
}
