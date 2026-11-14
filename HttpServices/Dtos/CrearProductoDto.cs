namespace VoltajeModa.HttpServices.Dtos;

public class CrearProductoDto
{
    public int CategoriaId { get; set; }
    public string SKU_Base { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioBase { get; set; }
    public string? UrlImagen { get; set; }
}
