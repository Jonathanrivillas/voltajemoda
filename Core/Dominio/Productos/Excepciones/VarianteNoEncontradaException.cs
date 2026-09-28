using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Productos.Excepciones;

public class VarianteNoEncontradaException : ExcepcionDominio
{
    public VarianteNoEncontradaException(int productoId, int varianteId)
        : base($"El producto {productoId} no tiene una variante con id {varianteId}.", 404)
    {
    }
}
