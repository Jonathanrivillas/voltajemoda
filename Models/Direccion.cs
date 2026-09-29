using System.ComponentModel.DataAnnotations;

namespace VoltajeModa.Models;

public class Direccion
{
    public int Id { get; set; }

    public string? UsuarioId { get; set; }
    public string? AnonimoId { get; set; }

    [StringLength(30, ErrorMessage = "La etiqueta no puede superar los 30 caracteres.")]
    public string Etiqueta { get; set; } = "Casa";

    [StringLength(100, ErrorMessage = "El nombre del destinatario no puede superar los 100 caracteres.")]
    public string? NombreDestinatario { get; set; }

    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "La calle/dirección es obligatoria.")]
    [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres.")]
    public string Calle { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es obligatoria.")]
    [StringLength(100, ErrorMessage = "La ciudad no puede superar los 100 caracteres.")]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código postal es obligatorio.")]
    [StringLength(20, ErrorMessage = "El código postal no puede superar los 20 caracteres.")]
    public string CodigoPostal { get; set; } = string.Empty;

    public bool EsPredeterminada { get; set; }
}
