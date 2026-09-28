using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Pedidos.Excepciones;

public class PedidoNoEliminableException : ExcepcionDominio
{
    public PedidoNoEliminableException()
        : base("Solo se pueden eliminar pedidos cancelados.", 409)
    {
    }
}
