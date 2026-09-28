using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Productos.Excepciones;

public class ProductoInvalidoException : ExcepcionDominio
{
    public ProductoInvalidoException(string mensaje) : base(mensaje, 400)
    {
    }
}
