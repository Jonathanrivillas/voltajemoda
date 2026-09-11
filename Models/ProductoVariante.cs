namespace VoltajeModa.Models;

public class ProductoVariante
{
    public int Id { get; set; }
    public string Talla { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Stock { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
}
