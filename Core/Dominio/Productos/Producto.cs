using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Dominio.Productos;

public class Producto
{
    private readonly List<ProductoVariante> _variantes = new();
    private readonly List<ProductoImagen> _imagenes = new();

    private Producto(
        int id, string nombre, string descripcion, decimal precio, string imagenUrl,
        bool enOferta, decimal? precioOferta, bool destacado, bool esNuevo, int categoriaId)
    {
        Id = id;
        Nombre = nombre;
        Descripcion = descripcion;
        Precio = precio;
        ImagenUrl = imagenUrl;
        EnOferta = enOferta;
        PrecioOferta = precioOferta;
        Destacado = destacado;
        EsNuevo = esNuevo;
        CategoriaId = categoriaId;
    }

    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public decimal Precio { get; private set; }
    public string ImagenUrl { get; private set; }
    public bool EnOferta { get; private set; }
    public decimal? PrecioOferta { get; private set; }
    public bool Destacado { get; private set; }
    public bool EsNuevo { get; private set; }
    public int CategoriaId { get; private set; }
    public IReadOnlyList<ProductoVariante> Variantes => _variantes.AsReadOnly();
    public IReadOnlyList<ProductoImagen> Imagenes => _imagenes.AsReadOnly();

    public static Producto Crear(
        string nombre, string descripcion, decimal precio, string imagenUrl,
        int categoriaId, bool destacado, bool esNuevo)
    {
        ValidarNombre(nombre);
        ValidarPrecio(precio);

        return new Producto(
            0, nombre.Trim(), descripcion ?? string.Empty, precio, imagenUrl ?? string.Empty,
            false, null, destacado, esNuevo, categoriaId);
    }

    internal static Producto Reconstituir(
        int id, string nombre, string descripcion, decimal precio, string imagenUrl,
        bool enOferta, decimal? precioOferta, bool destacado, bool esNuevo, int categoriaId,
        IEnumerable<ProductoVariante> variantes, IEnumerable<ProductoImagen> imagenes)
    {
        var producto = new Producto(id, nombre, descripcion, precio, imagenUrl, enOferta, precioOferta, destacado, esNuevo, categoriaId);
        producto._variantes.AddRange(variantes);
        producto._imagenes.AddRange(imagenes);
        return producto;
    }

    public void ActualizarDatos(
        string nombre, string descripcion, decimal precio, string imagenUrl,
        int categoriaId, bool destacado, bool esNuevo)
    {
        ValidarNombre(nombre);
        ValidarPrecio(precio);

        Nombre = nombre.Trim();
        Descripcion = descripcion ?? string.Empty;
        Precio = precio;
        ImagenUrl = imagenUrl ?? string.Empty;
        CategoriaId = categoriaId;
        Destacado = destacado;
        EsNuevo = esNuevo;

        if (EnOferta && PrecioOferta.HasValue && PrecioOferta.Value >= Precio)
        {
            EnOferta = false;
            PrecioOferta = null;
        }
    }

    public void PonerEnOferta(decimal precioOferta)
    {
        if (precioOferta <= 0)
        {
            throw new OfertaInvalidaException("El precio de oferta debe ser mayor a 0.");
        }

        if (precioOferta >= Precio)
        {
            throw new OfertaInvalidaException("El precio de oferta debe ser menor al precio de lista.");
        }

        EnOferta = true;
        PrecioOferta = precioOferta;
    }

    public void QuitarOferta()
    {
        EnOferta = false;
        PrecioOferta = null;
    }

    public decimal CalcularPrecioEfectivo() => EnOferta && PrecioOferta.HasValue ? PrecioOferta.Value : Precio;

    // Asignado por la infraestructura de persistencia una vez que EF Core genera el id real
    // (ver UnidadDeTrabajoEf / DbContext.SavedChanges en los repositorios).
    internal void AsignarId(int id) => Id = id;

    public ProductoVariante AgregarVariante(string talla, string color, int stockInicial)
    {
        if (string.IsNullOrWhiteSpace(talla) || string.IsNullOrWhiteSpace(color))
        {
            throw new ProductoInvalidoException("La talla y el color de la variante son obligatorios.");
        }

        if (stockInicial < 0)
        {
            throw new ProductoInvalidoException("El stock inicial no puede ser negativo.");
        }

        if (_variantes.Any(v =>
                string.Equals(v.Talla, talla, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(v.Color, color, StringComparison.OrdinalIgnoreCase)))
        {
            throw new VarianteDuplicadaException(talla, color);
        }

        var variante = new ProductoVariante(0, talla.Trim(), color.Trim(), stockInicial);
        _variantes.Add(variante);
        return variante;
    }

    public ProductoImagen AgregarImagen(string url, int orden)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ProductoInvalidoException("La url de la imagen es obligatoria.");
        }

        var imagen = new ProductoImagen(0, url.Trim(), orden);
        _imagenes.Add(imagen);
        return imagen;
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ProductoInvalidoException("Ingresa el nombre del producto.");
        }
    }

    private static void ValidarPrecio(decimal precio)
    {
        if (precio <= 0)
        {
            throw new ProductoInvalidoException("El precio debe ser mayor a 0.");
        }
    }
}
