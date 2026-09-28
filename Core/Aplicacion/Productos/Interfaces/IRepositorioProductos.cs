using VoltajeModa.Core.Aplicacion.Productos.Dtos;
using VoltajeModa.Core.Dominio.Productos;

namespace VoltajeModa.Core.Aplicacion.Productos.Interfaces;

public interface IRepositorioProductos
{
    Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<(IReadOnlyList<Producto> Items, int Total)> BuscarAsync(FiltroProductos filtro, CancellationToken ct);

    // No hace SaveChanges: el commit lo dispara IUnidadDeTrabajo al final del caso de uso.
    // El id real generado por la base de datos se refleja en producto.Id una vez completado
    // ese commit (ver Infraestructura/Persistencia/Productos/Repositorios/RepositorioProductosEf.cs).
    Task AgregarAsync(Producto producto, CancellationToken ct);
    Task ActualizarAsync(Producto producto, CancellationToken ct);
    Task EliminarAsync(int id, CancellationToken ct);
}
