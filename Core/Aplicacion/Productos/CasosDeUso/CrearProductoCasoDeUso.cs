using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class CrearProductoCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IRepositorioCategorias _categorias;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public CrearProductoCasoDeUso(IRepositorioProductos productos, IRepositorioCategorias categorias, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _categorias = categorias;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Producto> EjecutarAsync(
        string nombre, string descripcion, decimal precio, string imagenUrl,
        int categoriaId, bool destacado, bool esNuevo, CancellationToken ct)
    {
        _ = await _categorias.ObtenerPorIdAsync(categoriaId, ct) ?? throw new CategoriaNoEncontradaException(categoriaId);

        var producto = Producto.Crear(nombre, descripcion, precio, imagenUrl, categoriaId, destacado, esNuevo);
        await _productos.AgregarAsync(producto, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return producto;
    }
}
