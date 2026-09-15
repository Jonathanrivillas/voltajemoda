using Microsoft.EntityFrameworkCore;
using VoltajeModa.Data;
using VoltajeModa.Models;
using VoltajeModa.Services.Dtos;

namespace VoltajeModa.Services;

public class EstadisticasService(ApplicationDbContext db)
{
    public async Task<List<PuntoVentaDiariaDto>> ObtenerVentasPorRangoAsync(DateOnly desde, DateOnly hasta)
    {
        var desdeFecha = desde.ToDateTime(TimeOnly.MinValue);
        var hastaFecha = hasta.ToDateTime(TimeOnly.MaxValue);

        var ventas = await db.Pedidos
            .Where(p => p.Fecha >= desdeFecha && p.Fecha <= hastaFecha && p.Estado != EstadoPedido.Cancelado)
            .GroupBy(p => p.Fecha.Date)
            .Select(g => new { Fecha = g.Key, Total = g.Sum(p => p.Total) })
            .ToListAsync();

        var porFecha = ventas.ToDictionary(v => DateOnly.FromDateTime(v.Fecha), v => v.Total);

        var dias = hasta.DayNumber - desde.DayNumber + 1;
        var resultado = new List<PuntoVentaDiariaDto>();
        for (var i = 0; i < dias; i++)
        {
            var fecha = desde.AddDays(i);
            resultado.Add(new PuntoVentaDiariaDto(fecha, porFecha.GetValueOrDefault(fecha, 0m)));
        }

        return resultado;
    }

    private async Task<List<ProductoMasVendidoDto>> ConsultaVentasPorProductoAsync(DateOnly desde, DateOnly hasta)
    {
        var desdeFecha = desde.ToDateTime(TimeOnly.MinValue);
        var hastaFecha = hasta.ToDateTime(TimeOnly.MaxValue);

        // Se materializa primero (proyección plana, sin agrupar) porque EF Core no logra traducir
        // el Sum sobre un campo calculado cuando la clave de GroupBy viene de una navegación — el
        // GroupBy/Sum se resuelve en memoria sobre una lista ya chica (ítems de pedidos del rango).
        var itemsVendidos = await db.PedidoItems
            .Where(pi => pi.Pedido != null
                && pi.Pedido.Fecha >= desdeFecha && pi.Pedido.Fecha <= hastaFecha
                && pi.Pedido.Estado != EstadoPedido.Cancelado)
            .Select(pi => new
            {
                ProductoId = pi.ProductoVariante!.ProductoId,
                Nombre = pi.ProductoVariante.Producto!.Nombre,
                pi.Cantidad,
                Subtotal = pi.Cantidad * pi.PrecioUnitario
            })
            .ToListAsync();

        return itemsVendidos
            .GroupBy(x => new { x.ProductoId, x.Nombre })
            .Select(g => new ProductoMasVendidoDto(
                g.Key.ProductoId,
                g.Key.Nombre,
                g.Sum(x => x.Cantidad),
                g.Sum(x => x.Subtotal)))
            .ToList();
    }

    public async Task<List<ProductoMasVendidoDto>> ObtenerProductosMasVendidosAsync(DateOnly desde, DateOnly hasta, int cantidad = 10)
    {
        var productos = await ConsultaVentasPorProductoAsync(desde, hasta);
        return productos.OrderByDescending(p => p.UnidadesVendidas).Take(cantidad).ToList();
    }

    public async Task<List<ProductoMasVendidoDto>> ObtenerProductosMenosVendidosAsync(DateOnly desde, DateOnly hasta, int cantidad = 10)
    {
        var productos = await ConsultaVentasPorProductoAsync(desde, hasta);
        return productos.OrderBy(p => p.UnidadesVendidas).Take(cantidad).ToList();
    }

    public async Task<List<RendimientoCategoriaDto>> ObtenerRendimientoPorCategoriaAsync(DateOnly desde, DateOnly hasta)
    {
        var desdeFecha = desde.ToDateTime(TimeOnly.MinValue);
        var hastaFecha = hasta.ToDateTime(TimeOnly.MaxValue);

        var itemsVendidos = await db.PedidoItems
            .Where(pi => pi.Pedido != null
                && pi.Pedido.Fecha >= desdeFecha && pi.Pedido.Fecha <= hastaFecha
                && pi.Pedido.Estado != EstadoPedido.Cancelado)
            .Select(pi => new
            {
                CategoriaId = pi.ProductoVariante!.Producto!.CategoriaId,
                Nombre = pi.ProductoVariante.Producto.Categoria!.Nombre,
                pi.Cantidad,
                Subtotal = pi.Cantidad * pi.PrecioUnitario
            })
            .ToListAsync();

        return itemsVendidos
            .GroupBy(x => new { x.CategoriaId, x.Nombre })
            .Select(g => new RendimientoCategoriaDto(
                g.Key.CategoriaId,
                g.Key.Nombre,
                g.Sum(x => x.Cantidad),
                g.Sum(x => x.Subtotal)))
            .OrderByDescending(c => c.Ingresos)
            .ToList();
    }

    public async Task<Dictionary<EstadoPedido, int>> ObtenerDistribucionEstadosAsync(DateOnly desde, DateOnly hasta)
    {
        var desdeFecha = desde.ToDateTime(TimeOnly.MinValue);
        var hastaFecha = hasta.ToDateTime(TimeOnly.MaxValue);

        var conteos = await db.Pedidos
            .Where(p => p.Fecha >= desdeFecha && p.Fecha <= hastaFecha)
            .GroupBy(p => p.Estado)
            .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
            .ToListAsync();

        var resultado = Enum.GetValues<EstadoPedido>().ToDictionary(e => e, _ => 0);
        foreach (var c in conteos)
        {
            resultado[c.Estado] = c.Cantidad;
        }

        return resultado;
    }
}
