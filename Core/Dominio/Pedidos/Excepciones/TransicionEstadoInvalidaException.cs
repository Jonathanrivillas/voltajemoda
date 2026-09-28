using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Pedidos.Excepciones;

public class TransicionEstadoInvalidaException : ExcepcionDominio
{
    public TransicionEstadoInvalidaException(EstadoPedido estadoActual, EstadoPedido estadoDestino)
        : base($"No se puede cambiar el pedido de estado '{estadoActual}' a '{estadoDestino}'.", 409)
    {
    }
}
