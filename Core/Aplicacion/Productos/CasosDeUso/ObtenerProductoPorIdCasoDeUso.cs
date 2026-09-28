using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class ObtenerProductoPorIdCasoDeUso
{
    private readonly IRepositorioProductos _productos;

    public ObtenerProductoPorIdCasoDeUso(IRepositorioProductos productos)
    {
        _productos = productos;
    }

    public async Task<Producto> EjecutarAsync(int id, CancellationToken ct)
    {
        return await _productos.ObtenerPorIdAsync(id, ct) ?? throw new ProductoNoEncontradoException(id);
    }
}
