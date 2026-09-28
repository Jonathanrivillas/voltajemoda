using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Productos.Excepciones;

public class VarianteDuplicadaException : ExcepcionDominio
{
    public VarianteDuplicadaException(string talla, string color)
        : base($"Ya existe una variante con talla '{talla}' y color '{color}' para este producto.", 400)
    {
    }
}
