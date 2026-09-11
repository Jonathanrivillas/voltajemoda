namespace VoltajeModa.Models;

public class CarritoItem
{
    public int Id { get; set; }
    public int Cantidad { get; set; }

    public int CarritoId { get; set; }
    public Carrito? Carrito { get; set; }

    public int ProductoVarianteId { get; set; }
    public ProductoVariante? ProductoVariante { get; set; }
}
