using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Inventario.Excepciones;

public class StockInsuficienteException : ExcepcionDominio
{
    public StockInsuficienteException(int productoVarianteId, int stockActual, int cantidadSolicitada)
        : base(
            $"Stock insuficiente para la variante {productoVarianteId}: disponible {stockActual}, solicitado {cantidadSolicitada}.",
            409)
    {
    }
}
