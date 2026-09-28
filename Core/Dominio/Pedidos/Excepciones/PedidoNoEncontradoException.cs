using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Pedidos.Excepciones;

public class PedidoNoEncontradoException : ExcepcionDominio
{
    public PedidoNoEncontradoException(int id) : base($"No se encontró el pedido con id {id}.", 404)
    {
    }
}
