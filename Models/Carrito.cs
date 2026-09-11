namespace VoltajeModa.Models;

public class Carrito
{
    public int Id { get; set; }
    public string? UsuarioId { get; set; }
    public string? AnonimoId { get; set; }

    public List<CarritoItem> Items { get; set; } = new();
}
