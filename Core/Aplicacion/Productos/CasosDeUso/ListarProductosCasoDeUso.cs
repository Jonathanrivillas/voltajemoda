using VoltajeModa.Core.Aplicacion.Productos.Dtos;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class ListarProductosCasoDeUso
{
    private readonly IRepositorioProductos _productos;

    public ListarProductosCasoDeUso(IRepositorioProductos productos)
    {
        _productos = productos;
    }

    public Task<(IReadOnlyList<Producto> Items, int Total)> EjecutarAsync(FiltroProductos filtro, CancellationToken ct)
    {
        return _productos.BuscarAsync(filtro, ct);
    }
}
