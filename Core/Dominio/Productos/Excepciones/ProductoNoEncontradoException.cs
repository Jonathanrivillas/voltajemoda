using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Productos.Excepciones;

public class ProductoNoEncontradoException : ExcepcionDominio
{
    public ProductoNoEncontradoException(int id) : base($"No se encontró el producto con id {id}.", 404)
    {
    }
}
