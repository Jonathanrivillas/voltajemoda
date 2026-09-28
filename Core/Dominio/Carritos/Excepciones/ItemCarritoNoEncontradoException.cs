using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Carritos.Excepciones;

public class ItemCarritoNoEncontradoException : ExcepcionDominio
{
    public ItemCarritoNoEncontradoException(int productoVarianteId)
        : base($"El carrito no tiene un ítem para la variante {productoVarianteId}.", 404)
    {
    }
}
