using System.ComponentModel.DataAnnotations;

namespace VoltajeModa.Models;

public class Producto : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ingresa el nombre del producto.")]
    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;

    public bool EnOferta { get; set; }
    public bool Destacado { get; set; }
    public bool EsNuevo { get; set; }

    public decimal? PrecioOferta { get; set; }

    public decimal PrecioEfectivo => EnOferta && PrecioOferta is not null ? PrecioOferta.Value : Precio;

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public List<ProductoVariante> Variantes { get; set; } = new();
    public List<ProductoImagen> ImagenesAdicionales { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EnOferta)
        {
            if (PrecioOferta is null)
            {
                yield return new ValidationResult("Ingresa el precio de oferta.", new[] { nameof(PrecioOferta) });
            }
            else if (PrecioOferta >= Precio)
            {
                yield return new ValidationResult("El precio de oferta debe ser menor al precio de lista.", new[] { nameof(PrecioOferta) });
            }
        }
    }
}
