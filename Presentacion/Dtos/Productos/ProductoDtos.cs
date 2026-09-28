using VoltajeModa.Core.Dominio.Productos;

namespace VoltajeModa.Presentacion.Dtos.Productos;

public record VarianteDto(int Id, string Talla, string Color, int Stock)
{
    public static VarianteDto DesdeDominio(ProductoVariante v) => new(v.Id, v.Talla, v.Color, v.Stock);
}

public record ImagenDto(int Id, string Url, int Orden)
{
    public static ImagenDto DesdeDominio(ProductoImagen i) => new(i.Id, i.Url, i.Orden);
}

public record ProductoResumenDto(
    int Id, string Nombre, decimal Precio, decimal PrecioEfectivo, string ImagenUrl,
    bool EnOferta, bool Destacado, bool EsNuevo, int CategoriaId)
{
    public static ProductoResumenDto DesdeDominio(Producto p) => new(
        p.Id, p.Nombre, p.Precio, p.CalcularPrecioEfectivo(), p.ImagenUrl,
        p.EnOferta, p.Destacado, p.EsNuevo, p.CategoriaId);
}

public record ProductoDetalleDto(
    int Id, string Nombre, string Descripcion, decimal Precio, decimal? PrecioOferta, decimal PrecioEfectivo,
    string ImagenUrl, bool EnOferta, bool Destacado, bool EsNuevo, int CategoriaId,
    IReadOnlyList<VarianteDto> Variantes, IReadOnlyList<ImagenDto> Imagenes)
{
    public static ProductoDetalleDto DesdeDominio(Producto p) => new(
        p.Id, p.Nombre, p.Descripcion, p.Precio, p.PrecioOferta, p.CalcularPrecioEfectivo(),
        p.ImagenUrl, p.EnOferta, p.Destacado, p.EsNuevo, p.CategoriaId,
        p.Variantes.Select(VarianteDto.DesdeDominio).ToList(),
        p.Imagenes.Select(ImagenDto.DesdeDominio).ToList());
}

public record CrearProductoRequest(
    string Nombre, string Descripcion, decimal Precio, string ImagenUrl,
    int CategoriaId, bool Destacado, bool EsNuevo);

public record ActualizarProductoRequest(
    string Nombre, string Descripcion, decimal Precio, string ImagenUrl,
    int CategoriaId, bool Destacado, bool EsNuevo);

public record PonerOfertaRequest(decimal PrecioOferta);

public record CrearVarianteRequest(string Talla, string Color, int StockInicial);

public record CrearImagenRequest(string Url, int Orden);

public record CategoriaDto(int Id, string Nombre)
{
    public static CategoriaDto DesdeDominio(Categoria c) => new(c.Id, c.Nombre);
}

public record CrearCategoriaRequest(string Nombre);

public record ActualizarCategoriaRequest(string Nombre);
