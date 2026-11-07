namespace VoltajeModa.HttpServices.Dtos;

public class AuthResponseDto
{
    public string? Token { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Rol { get; set; }
    public DateTime Expiracion { get; set; }
}
