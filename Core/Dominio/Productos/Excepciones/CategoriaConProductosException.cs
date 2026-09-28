using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Productos.Excepciones;

public class CategoriaConProductosException : ExcepcionDominio
{
    public CategoriaConProductosException(string nombreCategoria, int cantidadProductos)
        : base($"No se puede eliminar '{nombreCategoria}': todavía tiene {cantidadProductos} producto(s) asociado(s).", 409)
    {
    }
}
