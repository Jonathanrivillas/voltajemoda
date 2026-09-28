using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Carritos.Interfaces;
using VoltajeModa.Data;

namespace VoltajeModa.Infraestructura.Persistencia.Carritos.Repositorios;

public class ConsultaVariantesCarritoEf : IConsultaVariantesCarrito
{
    private readonly ApplicationDbContext _context;

    public ConsultaVariantesCarritoEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExisteAsync(int productoVarianteId, CancellationToken ct) =>
        _context.ProductoVariantes.AnyAsync(v => v.Id == productoVarianteId, ct);
}
