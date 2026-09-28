using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Data;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Productos.Repositorios;

// Replica exactamente la validación de Components/Pages/Admin/ProductoForm.razor
// (EliminarVarianteConfirmada): los pedidos cancelados no bloquean el borrado, solo los activos.
public class VerificadorUsoVarianteEf : IVerificadorUsoVariante
{
    private readonly ApplicationDbContext _context;

    public VerificadorUsoVarianteEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EstaEnUsoAsync(int productoVarianteId, CancellationToken ct)
    {
        var tienePedidosActivos = await _context.PedidoItems.AnyAsync(
            pi => pi.ProductoVarianteId == productoVarianteId && pi.Pedido!.Estado != EfModels.EstadoPedido.Cancelado, ct);
        if (tienePedidosActivos)
        {
            return true;
        }

        var enCarrito = await _context.CarritoItems.AnyAsync(ci => ci.ProductoVarianteId == productoVarianteId, ct);
        if (enCarrito)
        {
            return true;
        }

        return await _context.MovimientosStock.AnyAsync(m => m.ProductoVarianteId == productoVarianteId, ct);
    }
}
