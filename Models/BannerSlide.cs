using System.ComponentModel.DataAnnotations;

namespace VoltajeModa.Models;

public class BannerSlide
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La URL de la imagen del banner es obligatoria.")]
    [StringLength(500, ErrorMessage = "La URL no puede superar los 500 caracteres.")]
    public string ImagenUrl { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
    public string? Titulo { get; set; }

    [StringLength(500, ErrorMessage = "La URL del enlace no puede superar los 500 caracteres.")]
    public string? EnlaceUrl { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El orden no puede ser negativo.")]
    public int Orden { get; set; }

    public bool Activo { get; set; } = true;
}
