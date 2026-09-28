using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Pedidos.Excepciones;

// Distinta de VoltajeModa.Core.Dominio.Inventario.Excepciones.StockInsuficienteException: esta la
// lanza el checkout de Pedidos a partir del mensaje que le devuelve el puerto IAjustadorStock
// (puerto angosto hacia Inventario), sin conocer los detalles internos de stock de esa variante.
public class StockInsuficienteException : ExcepcionDominio
{
    public StockInsuficienteException(string mensaje) : base(mensaje, 409)
    {
    }
}
