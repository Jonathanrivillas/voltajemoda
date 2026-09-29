using System.ComponentModel.DataAnnotations;

namespace VoltajeModa.Models;

public class ProductoVariante
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria.")]
    [StringLength(20, ErrorMessage = "La talla no puede superar los 20 caracteres.")]
    public string Talla { get; set; } = string.Empty;

    [Required(ErrorMessage = "El color es obligatorio.")]
    [StringLength(50, ErrorMessage = "El color no puede superar los 50 caracteres.")]
    public string Color { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
}
