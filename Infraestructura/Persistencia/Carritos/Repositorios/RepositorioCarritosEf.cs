using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Carritos.Interfaces;
using VoltajeModa.Core.Dominio.Carritos;
using VoltajeModa.Data;
using VoltajeModa.Infraestructura.Persistencia.Carritos.Mapeadores;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Carritos.Repositorios;

public class RepositorioCarritosEf : IRepositorioCarritos
{
    private readonly ApplicationDbContext _context;

    public RepositorioCarritosEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Carrito?> ObtenerPorUsuarioAsync(string usuarioId, CancellationToken ct)
    {
        var entidad = await Consulta().FirstOrDefaultAsync(c => c.UsuarioId == usuarioId, ct);
        return entidad is null ? null : CarritoMapeador.ADominio(entidad);
    }

    public async Task<Carrito?> ObtenerPorAnonimoAsync(string anonimoId, CancellationToken ct)
    {
        var entidad = await Consulta().FirstOrDefaultAsync(c => c.AnonimoId == anonimoId, ct);
        return entidad is null ? null : CarritoMapeador.ADominio(entidad);
    }

    public Task AgregarAsync(Carrito carrito, CancellationToken ct)
    {
        var entidad = CarritoMapeador.AEntidadNueva(carrito);
        _context.Carritos.Add(entidad);
        SuscribirAsignacionDeId(carrito, entidad);
        return Task.CompletedTask;
    }

    public async Task ActualizarAsync(Carrito carrito, CancellationToken ct)
    {
        var entidad = await Consulta().FirstOrDefaultAsync(c => c.Id == carrito.Id, ct);
        if (entidad is null)
        {
            return;
        }

        CarritoMapeador.VolcarEnEntidad(carrito, entidad);
    }

    public async Task EliminarAsync(int carritoId, CancellationToken ct)
    {
        var entidad = await _context.Carritos.FindAsync(new object?[] { carritoId }, ct);
        if (entidad is not null)
        {
            _context.Carritos.Remove(entidad);
        }
    }

    private IQueryable<EfModels.Carrito> Consulta() => _context.Carritos.Include(c => c.Items);

    private void SuscribirAsignacionDeId(Carrito carrito, EfModels.Carrito entidad)
    {
        void Handler(object? sender, SavedChangesEventArgs e)
        {
            carrito.AsignarId(entidad.Id);
            _context.SavedChanges -= Handler;
        }

        _context.SavedChanges += Handler;
    }
}
