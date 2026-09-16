namespace VoltajeModa.Models;

public class Direccion
{
    public int Id { get; set; }
    public string? UsuarioId { get; set; }
    public string? AnonimoId { get; set; }
    public string Etiqueta { get; set; } = "Casa";
    public string? NombreDestinatario { get; set; }
    public string? Telefono { get; set; }
    public string Calle { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public bool EsPredeterminada { get; set; }
}
