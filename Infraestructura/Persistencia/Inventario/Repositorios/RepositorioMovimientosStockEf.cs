using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Inventario.Dtos;
using VoltajeModa.Core.Aplicacion.Inventario.Interfaces;
using VoltajeModa.Core.Dominio.Inventario;
using VoltajeModa.Data;
using VoltajeModa.Infraestructura.Persistencia.Inventario.Mapeadores;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Inventario.Repositorios;

public class RepositorioMovimientosStockEf : IRepositorioMovimientosStock
{
    private readonly ApplicationDbContext _context;

    public RepositorioMovimientosStockEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AgregarAsync(MovimientoStock movimiento, CancellationToken ct)
    {
        var entidad = MovimientoStockMapeador.AEntidadNueva(movimiento);
        _context.MovimientosStock.Add(entidad);
        SuscribirAsignacionDeId(movimiento, entidad);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<MovimientoStock> Items, int Total)> ListarAsync(FiltroHistorial filtro, CancellationToken ct)
    {
        var consulta = _context.MovimientosStock.AsQueryable();

        if (filtro.ProductoVarianteId is int varianteId)
        {
            consulta = consulta.Where(m => m.ProductoVarianteId == varianteId);
        }

        if (filtro.Desde is DateTime desde)
        {
            consulta = consulta.Where(m => m.Fecha >= desde);
        }

        if (filtro.Hasta is DateTime hasta)
        {
            consulta = consulta.Where(m => m.Fecha <= hasta);
        }

        if (filtro.Tipo is TipoMovimiento tipo)
        {
            var tipoEf = (EfModels.TipoMovimientoStock)(int)tipo;
            consulta = consulta.Where(m => m.Tipo == tipoEf);
        }

        var total = await consulta.CountAsync(ct);
        var pagina = Math.Max(filtro.Pagina, 1);
        var tamanoPagina = Math.Clamp(filtro.TamanoPagina, 1, 100);

        var entidades = await consulta
            .OrderByDescending(m => m.Fecha)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return (entidades.Select(MovimientoStockMapeador.ADominio).ToList(), total);
    }

    private void SuscribirAsignacionDeId(MovimientoStock movimiento, EfModels.MovimientoStock entidad)
    {
        void Handler(object? sender, SavedChangesEventArgs e)
        {
            movimiento.AsignarId(entidad.Id);
            _context.SavedChanges -= Handler;
        }

        _context.SavedChanges += Handler;
    }
}
