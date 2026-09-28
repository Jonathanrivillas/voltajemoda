namespace VoltajeModa.Core.Dominio.Pedidos;

public class PedidoItem
{
    internal PedidoItem(int id, int productoVarianteId, int cantidad, decimal precioUnitario)
    {
        Id = id;
        ProductoVarianteId = productoVarianteId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }

    public int Id { get; private set; }
    public int ProductoVarianteId { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }

    public decimal CalcularSubtotal() => Cantidad * PrecioUnitario;
}
