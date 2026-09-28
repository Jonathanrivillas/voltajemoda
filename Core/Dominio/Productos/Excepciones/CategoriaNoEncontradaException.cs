using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Productos.Excepciones;

public class CategoriaNoEncontradaException : ExcepcionDominio
{
    public CategoriaNoEncontradaException(int id) : base($"No se encontró la categoría con id {id}.", 404)
    {
    }
}
