using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;
using VoltajeModa.Data;
using VoltajeModa.Infraestructura.Persistencia.Pedidos.Mapeadores;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Pedidos.Repositorios;

public class RepositorioDireccionesEf : IRepositorioDirecciones
{
    private readonly ApplicationDbContext _context;

    public RepositorioDireccionesEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Direccion?> ObtenerPorIdAsync(int id, CancellationToken ct)
    {
        var entidad = await _context.Direcciones.FirstOrDefaultAsync(d => d.Id == id, ct);
        return entidad is null ? null : DireccionMapeador.ADominio(entidad);
    }

    public async Task<IReadOnlyList<Direccion>> ListarPorTitularAsync(Titular titular, CancellationToken ct)
    {
        var consulta = titular.EsUsuario
            ? _context.Direcciones.Where(d => d.UsuarioId == titular.UsuarioId)
            : _context.Direcciones.Where(d => d.AnonimoId == titular.AnonimoId);

        var entidades = await consulta.ToListAsync(ct);
        return entidades.Select(DireccionMapeador.ADominio).ToList();
    }

    public Task AgregarAsync(Direccion direccion, CancellationToken ct)
    {
        var entidad = DireccionMapeador.AEntidadNueva(direccion);
        _context.Direcciones.Add(entidad);
        SuscribirAsignacionDeId(direccion, entidad);
        return Task.CompletedTask;
    }

    public async Task ActualizarAsync(Direccion direccion, CancellationToken ct)
    {
        var entidad = await _context.Direcciones.FirstOrDefaultAsync(d => d.Id == direccion.Id, ct);
        if (entidad is null)
        {
            return;
        }

        DireccionMapeador.VolcarEnEntidad(direccion, entidad);
    }

    public async Task EliminarAsync(int id, CancellationToken ct)
    {
        var entidad = await _context.Direcciones.FindAsync(new object?[] { id }, ct);
        if (entidad is not null)
        {
            _context.Direcciones.Remove(entidad);
        }
    }

    private void SuscribirAsignacionDeId(Direccion direccion, EfModels.Direccion entidad)
    {
        void Handler(object? sender, SavedChangesEventArgs e)
        {
            direccion.AsignarId(entidad.Id);
            _context.SavedChanges -= Handler;
        }

        _context.SavedChanges += Handler;
    }
}
