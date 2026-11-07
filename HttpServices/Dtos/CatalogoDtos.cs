using System.Text.Json.Serialization;
using VoltajeModa.Models;

namespace VoltajeModa.HttpServices.Dtos;

public class CatalogoPaginadoDto
{
    [JsonPropertyName("productos")]
    public List<ProductoDto> Productos { get; set; } = new();

    [JsonPropertyName("paginaActual")]
    public int PaginaActual { get; set; }

    [JsonPropertyName("totalPaginas")]
    public int TotalPaginas { get; set; }

    [JsonPropertyName("totalRegistros")]
    public int TotalRegistros { get; set; }
}

public class ProductoDto
{
    [JsonPropertyName("productoId")]
    public int ProductoId { get; set; }

    // Compatibilidad con vistas que usan Id
    public int Id => ProductoId;

    [JsonPropertyName("categoriaId")]
    public int CategoriaId { get; set; }

    [JsonPropertyName("nombreCategoria")]
    public string NombreCategoria { get; set; } = string.Empty;

    [JsonPropertyName("skuBase")]
    public string SkuBase { get; set; } = string.Empty;

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("descripcion")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("precioBase")]
    public decimal PrecioBase { get; set; }

    // Compatibilidad de precios
    public decimal Precio => PrecioBase;
    public decimal PrecioEfectivo => PrecioBase;

    [JsonPropertyName("activo")]
    public bool Activo { get; set; }

    public bool EnOferta { get; set; }
    public decimal? PrecioOferta { get; set; }
    public bool EsNuevo { get; set; }

    [JsonPropertyName("imagenes")]
    public List<ImagenDto> Imagenes { get; set; } = new();

    [JsonPropertyName("variantes")]
    public List<VarianteDto> Variantes { get; set; } = new();

    public string ImagenUrl => Imagenes.FirstOrDefault()?.UrlImagen ?? string.Empty;

    public List<ProductoImagen> ImagenesAdicionales => Imagenes
        .OrderBy(i => i.Orden)
        .Select(i => new ProductoImagen { Id = i.ImagenId, Url = i.UrlImagen, Orden = i.Orden })
        .ToList();

    public Producto AModelo()
    {
        return new Producto
        {
            Id = ProductoId,
            Nombre = Nombre,
            Descripcion = Descripcion ?? string.Empty,
            Precio = PrecioBase,
            CategoriaId = CategoriaId,
            Categoria = new Categoria { Id = CategoriaId, Nombre = NombreCategoria },
            ImagenUrl = ImagenUrl,
            EnOferta = EnOferta,
            PrecioOferta = PrecioOferta,
            EsNuevo = EsNuevo,
            Variantes = Variantes.Where(v => v.Activo).Select(v => new ProductoVariante
            {
                Id = v.VarianteId,
                ProductoId = ProductoId,
                Talla = v.Talla ?? string.Empty,
                Color = v.Color ?? string.Empty,
                Stock = v.Stock
            }).ToList(),
            ImagenesAdicionales = Imagenes.Select(i => new ProductoImagen
            {
                Id = i.ImagenId,
                ProductoId = ProductoId,
                Url = i.UrlImagen,
                Orden = i.Orden
            }).ToList()
        };
    }
}

public class CategoriaDto
{
    [JsonPropertyName("categoriaId")]
    public int CategoriaId { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("activo")]
    public bool Activo { get; set; }
}

public class BannerDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string Imagen { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string ImagenUrl => Imagen;
    public string EnlaceUrl => $"/productos/{Id}";
}

public class ProductoDetalleDto : ProductoDto
{
}

public class VarianteDto
{
    [JsonPropertyName("varianteId")]
    public int VarianteId { get; set; }

    [JsonPropertyName("sku_Variante")]
    public string SkuVariante { get; set; } = string.Empty;

    [JsonPropertyName("talla")]
    public string? Talla { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("stock")]
    public int Stock { get; set; }

    [JsonPropertyName("activo")]
    public bool Activo { get; set; }
}

public class ImagenDto
{
    [JsonPropertyName("imagenId")]
    public int ImagenId { get; set; }

    [JsonPropertyName("productoId")]
    public int ProductoId { get; set; }

    [JsonPropertyName("urlImagen")]
    public string UrlImagen { get; set; } = string.Empty;

    [JsonPropertyName("orden")]
    public int Orden { get; set; }
}
