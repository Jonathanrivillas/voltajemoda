using VoltajeModa.Core.Dominio.Comun.ValueObjects;

namespace VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;

// Puerto angosto hacia el bounded context de Carrito: el checkout solo necesita las líneas
// (variante + cantidad), no el agregado Carrito completo.
public interface ILectorCarritoParaCheckout
{
    Task<IReadOnlyList<(int ProductoVarianteId, int Cantidad)>> ObtenerLineasAsync(Titular titular, CancellationToken ct);
}
