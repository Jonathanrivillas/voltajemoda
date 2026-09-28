using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Productos.Excepciones;

public class VarianteEnUsoException : ExcepcionDominio
{
    public VarianteEnUsoException()
        : base("No se puede eliminar esta variante: está en un carrito, tiene pedidos activos (no cancelados) o movimientos de stock registrados.", 409)
    {
    }
}
