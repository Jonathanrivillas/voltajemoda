using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Productos.Dtos;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos;
using VoltajeModa.Data;
using VoltajeModa.Infraestructura.Persistencia.Productos.Mapeadores;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Productos.Repositorios;

public class RepositorioProductosEf : IRepositorioProductos
{
    private readonly ApplicationDbContext _context;

    public RepositorioProductosEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken ct)
    {
        var entidad = await Consulta().FirstOrDefaultAsync(p => p.Id == id, ct);
        return entidad is null ? null : ProductoMapeador.ADominio(entidad);
    }

    public async Task<(IReadOnlyList<Producto> Items, int Total)> BuscarAsync(FiltroProductos filtro, CancellationToken ct)
    {
        var consulta = Consulta();

        if (filtro.CategoriaId is int categoriaId)
        {
            consulta = consulta.Where(p => p.CategoriaId == categoriaId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
        {
            consulta = consulta.Where(p => p.Nombre.Contains(filtro.Busqueda));
        }

        if (filtro.Destacado is bool destacado)
        {
            consulta = consulta.Where(p => p.Destacado == destacado);
        }

        if (filtro.EnOferta is bool enOferta)
        {
            consulta = consulta.Where(p => p.EnOferta == enOferta);
        }

        if (filtro.EsNuevo is bool esNuevo)
        {
            consulta = consulta.Where(p => p.EsNuevo == esNuevo);
        }

        var total = await consulta.CountAsync(ct);
        var pagina = Math.Max(filtro.Pagina, 1);
        var tamanoPagina = Math.Clamp(filtro.TamanoPagina, 1, 100);

        var entidades = await consulta
            .OrderBy(p => p.Nombre)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return (entidades.Select(ProductoMapeador.ADominio).ToList(), total);
    }

    public Task AgregarAsync(Producto producto, CancellationToken ct)
    {
        var entidad = ProductoMapeador.AEntidadNueva(producto);
        _context.Productos.Add(entidad);
        SuscribirAsignacionDeId(producto, entidad);
        return Task.CompletedTask;
    }

    public async Task ActualizarAsync(Producto producto, CancellationToken ct)
    {
        var entidad = await Consulta().FirstOrDefaultAsync(p => p.Id == producto.Id, ct);
        if (entidad is null)
        {
            return;
        }

        ProductoMapeador.VolcarEnEntidad(producto, entidad);
    }

    public async Task EliminarAsync(int id, CancellationToken ct)
    {
        var entidad = await _context.Productos.FindAsync(new object?[] { id }, ct);
        if (entidad is not null)
        {
            _context.Productos.Remove(entidad);
        }
    }

    private IQueryable<EfModels.Producto> Consulta() =>
        _context.Productos.Include(p => p.Variantes).Include(p => p.ImagenesAdicionales);

    private void SuscribirAsignacionDeId(Producto producto, EfModels.Producto entidad)
    {
        void Handler(object? sender, SavedChangesEventArgs e)
        {
            producto.AsignarId(entidad.Id);
            _context.SavedChanges -= Handler;
        }

        _context.SavedChanges += Handler;
    }
}
