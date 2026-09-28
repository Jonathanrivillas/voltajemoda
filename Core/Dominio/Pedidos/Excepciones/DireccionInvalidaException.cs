using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Pedidos.Excepciones;

public class DireccionInvalidaException : ExcepcionDominio
{
    public DireccionInvalidaException(string mensaje) : base(mensaje, 400)
    {
    }
}
