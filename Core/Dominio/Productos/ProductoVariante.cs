namespace VoltajeModa.Core.Dominio.Productos;

// El Stock es de solo lectura desde aquí: el único dueño de su mutación es el bounded context
// de Inventario (RegistrarMovimientoStockCasoDeUso), incluso cuando el origen es una venta.
public class ProductoVariante
{
    internal ProductoVariante(int id, string talla, string color, int stock)
    {
        Id = id;
        Talla = talla;
        Color = color;
        Stock = stock;
    }

    public int Id { get; private set; }
    public string Talla { get; private set; }
    public string Color { get; private set; }
    public int Stock { get; private set; }
}
