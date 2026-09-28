using VoltajeModa.Core.Dominio.Carritos;

namespace VoltajeModa.Core.Aplicacion.Carritos.Interfaces;

public interface IRepositorioCarritos
{
    Task<Carrito?> ObtenerPorUsuarioAsync(string usuarioId, CancellationToken ct);
    Task<Carrito?> ObtenerPorAnonimoAsync(string anonimoId, CancellationToken ct);
    Task AgregarAsync(Carrito carrito, CancellationToken ct);
    Task ActualizarAsync(Carrito carrito, CancellationToken ct);
    Task EliminarAsync(int carritoId, CancellationToken ct);
}
