using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Inventario.Dtos;
using VoltajeModa.Core.Aplicacion.Inventario.Interfaces;
using VoltajeModa.Core.Dominio.Inventario;
using VoltajeModa.Data;

namespace VoltajeModa.Infraestructura.Persistencia.Inventario.Repositorios;

public class ConsultaVariantesInventarioEf : IConsultaVariantesInventario
{
    private readonly ApplicationDbContext _context;

    public ConsultaVariantesInventarioEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int?> ObtenerStockActualAsync(int productoVarianteId, CancellationToken ct)
    {
        return _context.ProductoVariantes
            .Where(v => v.Id == productoVarianteId)
            .Select(v => (int?)v.Stock)
            .FirstOrDefaultAsync(ct);
    }

    public async Task ActualizarStockAsync(int productoVarianteId, int nuevoStock, CancellationToken ct)
    {
        var variante = await _context.ProductoVariantes.FirstOrDefaultAsync(v => v.Id == productoVarianteId, ct);
        if (variante is not null)
        {
            variante.Stock = nuevoStock;
        }
    }

    public async Task<IReadOnlyList<VarianteInventarioLectura>> ListarAsync(FiltroVariantes filtro, CancellationToken ct)
    {
        var consulta = _context.ProductoVariantes
            .Include(v => v.Producto!).ThenInclude(p => p!.Categoria)
            .AsQueryable();

        if (filtro.CategoriaId is int categoriaId)
        {
            consulta = consulta.Where(v => v.Producto!.CategoriaId == categoriaId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
        {
            consulta = consulta.Where(v => v.Producto!.Nombre.Contains(filtro.Busqueda));
        }

        if (filtro.Estado is EstadoStock estado)
        {
            consulta = estado switch
            {
                EstadoStock.SinStock => consulta.Where(v => v.Stock <= 0),
                EstadoStock.StockBajo => consulta.Where(v => v.Stock > 0 && v.Stock <= PoliticaStock.UmbralStockBajo),
                EstadoStock.EnStock => consulta.Where(v => v.Stock > PoliticaStock.UmbralStockBajo),
                _ => consulta
            };
        }

        var entidades = await consulta.OrderBy(v => v.Producto!.Nombre).ToListAsync(ct);

        return entidades.Select(v => new VarianteInventarioLectura(
            v.Id, v.ProductoId, v.Producto!.Nombre, v.Producto.ImagenUrl, v.Talla, v.Color, v.Stock,
            v.Producto.CategoriaId, v.Producto.Categoria?.Nombre ?? string.Empty)).ToList();
    }
}
