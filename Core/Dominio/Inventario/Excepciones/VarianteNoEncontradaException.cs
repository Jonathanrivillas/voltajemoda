using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Inventario.Excepciones;

public class VarianteNoEncontradaException : ExcepcionDominio
{
    public VarianteNoEncontradaException(int productoVarianteId)
        : base($"No se encontró la variante de producto con id {productoVarianteId}.", 404)
    {
    }
}
