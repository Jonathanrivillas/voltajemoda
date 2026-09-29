using System.ComponentModel.DataAnnotations;

namespace VoltajeModa.Models;

public class CarritoItem
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }

    public int CarritoId { get; set; }
    public Carrito? Carrito { get; set; }

    public int ProductoVarianteId { get; set; }
    public ProductoVariante? ProductoVariante { get; set; }
}
