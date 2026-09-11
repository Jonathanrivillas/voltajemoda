namespace VoltajeModa.Models;

public class Carrito
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;

    public List<CarritoItem> Items { get; set; } = new();
}
