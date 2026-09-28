namespace VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;

// Puerto angosto hacia el bounded context de Productos: solo existencia + precio efectivo vigente
// (se toma como snapshot al momento del checkout, no como referencia futura al producto).
public interface IConsultaVarianteCheckout
{
    Task<(bool Existe, decimal PrecioUnitario)> ObtenerAsync(int productoVarianteId, CancellationToken ct);
}
