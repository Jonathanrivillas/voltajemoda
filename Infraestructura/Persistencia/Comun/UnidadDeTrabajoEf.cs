using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Data;

namespace VoltajeModa.Infraestructura.Persistencia.Comun;

public class UnidadDeTrabajoEf : IUnidadDeTrabajo
{
    private readonly ApplicationDbContext _context;

    public UnidadDeTrabajoEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}
