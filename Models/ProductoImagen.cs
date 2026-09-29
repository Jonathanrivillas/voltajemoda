using System.ComponentModel.DataAnnotations;

namespace VoltajeModa.Models;

public class ProductoImagen
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
    [StringLength(500, ErrorMessage = "La URL no puede superar los 500 caracteres.")]
    public string Url { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "El orden no puede ser negativo.")]
    public int Orden { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
}
