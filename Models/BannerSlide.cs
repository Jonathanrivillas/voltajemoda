namespace VoltajeModa.Models;

public class BannerSlide
{
    public int Id { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
    public string? Titulo { get; set; }
    public string? EnlaceUrl { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;
}
