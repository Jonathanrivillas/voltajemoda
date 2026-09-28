using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class AgregarVarianteCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public AgregarVarianteCasoDeUso(IRepositorioProductos productos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<ProductoVariante> EjecutarAsync(int productoId, string talla, string color, int stockInicial, CancellationToken ct)
    {
        var producto = await _productos.ObtenerPorIdAsync(productoId, ct) ?? throw new ProductoNoEncontradoException(productoId);

        producto.AgregarVariante(talla, color, stockInicial);
        await _productos.ActualizarAsync(producto, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        // Tras el commit, la variante recién agregada ya tiene su id real asignado por la base
        // de datos; se recupera releyendo el agregado para no acoplar el repositorio a devolver
        // ids individuales de entidades hijas.
        var productoActualizado = await _productos.ObtenerPorIdAsync(productoId, ct) ?? throw new ProductoNoEncontradoException(productoId);
        return productoActualizado.Variantes.Single(v =>
            string.Equals(v.Talla, talla.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(v.Color, color.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
