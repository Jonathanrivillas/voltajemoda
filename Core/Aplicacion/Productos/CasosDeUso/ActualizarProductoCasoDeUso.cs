using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class ActualizarProductoCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IRepositorioCategorias _categorias;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ActualizarProductoCasoDeUso(IRepositorioProductos productos, IRepositorioCategorias categorias, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _categorias = categorias;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Producto> EjecutarAsync(
        int id, string nombre, string descripcion, decimal precio, string imagenUrl,
        int categoriaId, bool destacado, bool esNuevo, CancellationToken ct)
    {
        var producto = await _productos.ObtenerPorIdAsync(id, ct) ?? throw new ProductoNoEncontradoException(id);
        _ = await _categorias.ObtenerPorIdAsync(categoriaId, ct) ?? throw new CategoriaNoEncontradaException(categoriaId);

        producto.ActualizarDatos(nombre, descripcion, precio, imagenUrl, categoriaId, destacado, esNuevo);
        await _productos.ActualizarAsync(producto, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return producto;
    }
}
