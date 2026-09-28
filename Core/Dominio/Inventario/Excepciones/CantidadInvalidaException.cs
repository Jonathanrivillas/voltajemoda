using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Inventario.Excepciones;

public class CantidadInvalidaException : ExcepcionDominio
{
    public CantidadInvalidaException(string mensaje) : base(mensaje, 400)
    {
    }
}
