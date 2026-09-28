using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Productos.Excepciones;

public class ImagenNoEncontradaException : ExcepcionDominio
{
    public ImagenNoEncontradaException(int productoId, int imagenId)
        : base($"El producto {productoId} no tiene una imagen con id {imagenId}.", 404)
    {
    }
}
