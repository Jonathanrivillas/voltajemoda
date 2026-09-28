using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Pedidos.Excepciones;

public class PedidoVacioException : ExcepcionDominio
{
    public PedidoVacioException() : base("El pedido no puede estar vacío.", 409)
    {
    }
}
