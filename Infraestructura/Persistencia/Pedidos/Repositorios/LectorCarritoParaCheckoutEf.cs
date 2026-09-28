using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Data;

namespace VoltajeModa.Infraestructura.Persistencia.Pedidos.Repositorios;

// Implementa el puerto angosto hacia Carrito: solo lee líneas (variante + cantidad) directo del
// modelo EF de Carrito, sin pasar por el agregado de dominio Carrito ni por su repositorio.
public class LectorCarritoParaCheckoutEf : ILectorCarritoParaCheckout
{
    private readonly ApplicationDbContext _context;

    public LectorCarritoParaCheckoutEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<(int ProductoVarianteId, int Cantidad)>> ObtenerLineasAsync(Titular titular, CancellationToken ct)
    {
        var consulta = _context.Carritos.Include(c => c.Items).AsQueryable();

        var carrito = titular.EsUsuario
            ? await consulta.FirstOrDefaultAsync(c => c.UsuarioId == titular.UsuarioId, ct)
            : await consulta.FirstOrDefaultAsync(c => c.AnonimoId == titular.AnonimoId, ct);

        return carrito?.Items.Select(i => (i.ProductoVarianteId, i.Cantidad)).ToList()
            ?? new List<(int, int)>();
    }
}
