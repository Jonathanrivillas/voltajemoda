using Microsoft.EntityFrameworkCore;
using VoltajeModa.Data;
using VoltajeModa.Models;
using VoltajeModa.Services.Dtos;

namespace VoltajeModa.Services;

public class DashboardService(ApplicationDbContext db)
{
    public async Task<KpiResumenDto> ObtenerResumenAsync()
    {
        var ahora = DateTime.UtcNow;
        var inicioHoy = ahora.Date;
        var inicioMes = new DateTime(ahora.Year, ahora.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var ingresosHoy = await db.Pedidos
            .Where(p => p.Fecha >= inicioHoy && p.Estado != EstadoPedido.Cancelado)
            .SumAsync(p => (decimal?)p.Total) ?? 0m;

        var ingresosMes = await db.Pedidos
            .Where(p => p.Fecha >= inicioMes && p.Estado != EstadoPedido.Cancelado)
            .SumAsync(p => (decimal?)p.Total) ?? 0m;

        var pedidosHoy = await db.Pedidos.CountAsync(p => p.Fecha >= inicioHoy);
        var pedidosPendientes = await db.Pedidos.CountAsync(p => p.Estado == EstadoPedido.Pendiente);

        // Cuenta variantes en umbral de stock bajo (no productos), ver InventarioService.UmbralStockBajo.
        var variantesBajoStock = await db.ProductoVariantes
            .CountAsync(v => v.Stock <= InventarioService.UmbralStockBajo);

        var totalProductos = await db.Productos.CountAsync();
        var totalCategorias = await db.Categorias.CountAsync();

        return new KpiResumenDto(
            ingresosHoy,
            ingresosMes,
            pedidosHoy,
            pedidosPendientes,
            variantesBajoStock,
            totalProductos,
            totalCategorias);
    }

    public async Task<List<PuntoVentaDiariaDto>> ObtenerVentasUltimosDiasAsync(int dias = 30)
    {
        var desde = DateTime.UtcNow.Date.AddDays(-(dias - 1));

        var ventas = await db.Pedidos
            .Where(p => p.Fecha >= desde && p.Estado != EstadoPedido.Cancelado)
            .GroupBy(p => p.Fecha.Date)
            .Select(g => new { Fecha = g.Key, Total = g.Sum(p => p.Total) })
            .ToListAsync();

        var porFecha = ventas.ToDictionary(v => DateOnly.FromDateTime(v.Fecha), v => v.Total);

        var resultado = new List<PuntoVentaDiariaDto>();
        for (var i = 0; i < dias; i++)
        {
            var fecha = DateOnly.FromDateTime(desde.AddDays(i));
            resultado.Add(new PuntoVentaDiariaDto(fecha, porFecha.GetValueOrDefault(fecha, 0m)));
        }

        return resultado;
    }

    public async Task<List<ProductoMasVendidoDto>> ObtenerTopProductosAsync(int cantidad = 5)
    {
        // Se materializa primero (proyección plana, sin agrupar) porque EF Core no logra traducir
        // el Sum sobre un campo calculado cuando la clave de GroupBy viene de una navegación — el
        // GroupBy/Sum se resuelve en memoria sobre una lista ya chica (ítems de pedidos vendidos).
        var itemsVendidos = await db.PedidoItems
            .Where(pi => pi.Pedido != null && pi.Pedido.Estado != EstadoPedido.Cancelado)
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
            .OrderByDescending(p => p.UnidadesVendidas)
            .Take(cantidad)
            .ToList();
    }

    public async Task<List<PedidoRecienteDto>> ObtenerPedidosRecientesAsync(int cantidad = 5)
    {
        var pedidos = await db.Pedidos
            .OrderByDescending(p => p.Fecha)
            .Take(cantidad)
            .ToListAsync();

        var usuarioIds = pedidos.Where(p => p.UsuarioId is not null).Select(p => p.UsuarioId!).Distinct().ToList();
        var usuarios = await db.Users
            .Where(u => usuarioIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? u.UserName ?? u.Id);

        return pedidos
            .Select(p => new PedidoRecienteDto(
                p.Id,
                p.Fecha,
                p.UsuarioId is not null ? usuarios.GetValueOrDefault(p.UsuarioId, "?") : "Invitado",
                p.Total,
                p.Estado))
            .ToList();
    }

    public async Task<List<AlertaStockDto>> ObtenerAlertasStockAsync(int cantidad = 8)
    {
        return await db.ProductoVariantes
            .Where(v => v.Stock <= InventarioService.UmbralStockBajo)
            .OrderBy(v => v.Stock)
            .Take(cantidad)
            .Select(v => new AlertaStockDto(v.Id, v.Producto!.Nombre, v.Talla, v.Color, v.Stock))
            .ToListAsync();
    }
}
