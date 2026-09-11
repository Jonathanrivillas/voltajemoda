using Microsoft.EntityFrameworkCore;

namespace VoltajeModa.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    [Precision(18, 2)]
    public decimal Precio { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public List<ProductoVariante> Variantes { get; set; } = new();
}
