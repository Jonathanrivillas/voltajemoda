namespace VoltajeModa.Core.Dominio.Carritos;

public class ItemCarrito
{
    internal ItemCarrito(int id, int productoVarianteId, int cantidad)
    {
        Id = id;
        ProductoVarianteId = productoVarianteId;
        Cantidad = cantidad;
    }

    public int Id { get; private set; }
    public int ProductoVarianteId { get; private set; }
    public int Cantidad { get; private set; }

    internal void SumarCantidad(int cantidad) => Cantidad += cantidad;
    internal void FijarCantidad(int cantidad) => Cantidad = cantidad;
}
