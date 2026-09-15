namespace VoltajeModa.Models;

public class ProductoImagen
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int Orden { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
}
