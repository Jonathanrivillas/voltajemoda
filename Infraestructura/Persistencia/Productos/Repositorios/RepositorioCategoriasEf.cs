using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Data;
using VoltajeModa.Infraestructura.Persistencia.Productos.Mapeadores;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Productos.Repositorios;

public class RepositorioCategoriasEf : IRepositorioCategorias
{
    private readonly ApplicationDbContext _context;

    public RepositorioCategoriasEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Categoria?> ObtenerPorIdAsync(int id, CancellationToken ct)
    {
        var entidad = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id, ct);
        return entidad is null ? null : CategoriaMapeador.ADominio(entidad);
    }

    public async Task<IReadOnlyList<Categoria>> ListarTodasAsync(CancellationToken ct)
    {
        var entidades = await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(ct);
        return entidades.Select(CategoriaMapeador.ADominio).ToList();
    }

    public Task AgregarAsync(Categoria categoria, CancellationToken ct)
    {
        var entidad = CategoriaMapeador.AEntidadNueva(categoria);
        _context.Categorias.Add(entidad);
        SuscribirAsignacionDeId(categoria, entidad);
        return Task.CompletedTask;
    }

    public async Task ActualizarAsync(Categoria categoria, CancellationToken ct)
    {
        var entidad = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == categoria.Id, ct);
        if (entidad is not null)
        {
            entidad.Nombre = categoria.Nombre;
        }
    }

    public async Task EliminarAsync(int id, CancellationToken ct)
    {
        var entidad = await _context.Categorias.FindAsync(new object?[] { id }, ct);
        if (entidad is not null)
        {
            _context.Categorias.Remove(entidad);
        }
    }

    public Task<int> ContarProductosAsync(int categoriaId, CancellationToken ct) =>
        _context.Productos.CountAsync(p => p.CategoriaId == categoriaId, ct);

    private void SuscribirAsignacionDeId(Categoria categoria, EfModels.Categoria entidad)
    {
        void Handler(object? sender, SavedChangesEventArgs e)
        {
            categoria.AsignarId(entidad.Id);
            _context.SavedChanges -= Handler;
        }

        _context.SavedChanges += Handler;
    }
}
