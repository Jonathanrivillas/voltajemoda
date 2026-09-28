using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Productos.Excepciones;

public class OfertaInvalidaException : ExcepcionDominio
{
    public OfertaInvalidaException(string mensaje) : base(mensaje, 400)
    {
    }
}
