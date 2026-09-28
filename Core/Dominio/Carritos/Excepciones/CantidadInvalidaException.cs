using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Carritos.Excepciones;

public class CantidadInvalidaException : ExcepcionDominio
{
    public CantidadInvalidaException(string mensaje) : base(mensaje, 400)
    {
    }
}
