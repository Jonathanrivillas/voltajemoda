using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Carritos.Excepciones;

public class CarritoNoEncontradoException : ExcepcionDominio
{
    public CarritoNoEncontradoException(int id) : base($"No se encontró el carrito con id {id}.", 404)
    {
    }
}
