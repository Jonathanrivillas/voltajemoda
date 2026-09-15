using Microsoft.EntityFrameworkCore;
using VoltajeModa.Data;
using VoltajeModa.Models;
using VoltajeModa.Services.Dtos;

namespace VoltajeModa.Services;

public class InventarioService(ApplicationDbContext db)
{
    public const int UmbralStockBajo = 5;

    public async Task<List<VarianteInventarioDto>> ObtenerVariantesAsync(
        int? categoriaId, string? busqueda, EstadoStockFiltro estado)
    {
        var query = db.ProductoVariantes
            .Include(v => v.Producto)
            .ThenInclude(p => p!.Categoria)
            .AsQueryable();

        if (categoriaId is not null)
        {
            query = query.Where(v => v.Producto!.CategoriaId == categoriaId);
        }

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();
            query = query.Where(v => v.Producto!.Nombre.Contains(termino));
        }

        query = estado switch
        {
            EstadoStockFiltro.SinStock => query.Where(v => v.Stock == 0),
            EstadoStockFiltro.StockBajo => query.Where(v => v.Stock > 0 && v.Stock <= UmbralStockBajo),
            EstadoStockFiltro.EnStock => query.Where(v => v.Stock > UmbralStockBajo),
            _ => query
        };

        return await query
            .OrderBy(v => v.Producto!.Nombre)
            .ThenBy(v => v.Talla)
            .Select(v => new VarianteInventarioDto(
                v.Id,
                v.ProductoId,
                v.Producto!.Nombre,
                v.Producto.ImagenUrl,
                v.Talla,
                v.Color,
                v.Stock,
                v.Producto.CategoriaId,
                v.Producto.Categoria!.Nombre))
            .ToListAsync();
    }

    public async Task<ResultadoMovimientoStock> RegistrarMovimientoAsync(
        int productoVarianteId, TipoMovimientoStock tipo, int cantidad, string motivo, string? usuarioId)
    {
        if (cantidad <= 0)
        {
            return new ResultadoMovimientoStock(false, "La cantidad debe ser mayor a cero.", null);
        }

        var variante = await db.ProductoVariantes.FirstOrDefaultAsync(v => v.Id == productoVarianteId);
        if (variante is null)
        {
            return new ResultadoMovimientoStock(false, "La variante no existe.", null);
        }

        if (tipo == TipoMovimientoStock.Salida && cantidad > variante.Stock)
        {
            return new ResultadoMovimientoStock(false, "No hay suficiente stock para registrar esta salida.", null);
        }

        variante.Stock += tipo == TipoMovimientoStock.Entrada ? cantidad : -cantidad;

        var movimiento = new MovimientoStock
        {
            ProductoVarianteId = productoVarianteId,
            Tipo = tipo,
            Cantidad = cantidad,
            Motivo = motivo,
            UsuarioId = usuarioId,
            StockResultante = variante.Stock
        };

        db.MovimientosStock.Add(movimiento);
        await db.SaveChangesAsync();

        return new ResultadoMovimientoStock(true, null, movimiento);
    }

    public async Task<(List<MovimientoStock> Movimientos, int Total)> ObtenerHistorialAsync(
        int? productoVarianteId, DateTime? desde, DateTime? hasta, TipoMovimientoStock? tipo,
        int pagina, int tamanoPagina)
    {
        var query = db.MovimientosStock
            .Include(m => m.ProductoVariante)
            .ThenInclude(v => v!.Producto)
            .AsQueryable();

        if (productoVarianteId is not null)
        {
            query = query.Where(m => m.ProductoVarianteId == productoVarianteId);
        }

        if (desde is not null)
        {
            query = query.Where(m => m.Fecha >= desde);
        }

        if (hasta is not null)
        {
            query = query.Where(m => m.Fecha <= hasta);
        }

        if (tipo is not null)
        {
            query = query.Where(m => m.Tipo == tipo);
        }

        query = query.OrderByDescending(m => m.Fecha);

        var total = await query.CountAsync();
        var movimientos = await query
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return (movimientos, total);
    }
}
