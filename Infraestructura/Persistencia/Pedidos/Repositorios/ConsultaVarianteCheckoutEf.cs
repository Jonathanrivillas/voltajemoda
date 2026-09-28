using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Data;

namespace VoltajeModa.Infraestructura.Persistencia.Pedidos.Repositorios;

// Implementa el puerto angosto hacia Productos: solo existencia + precio efectivo vigente, tomado
// como snapshot en el momento del checkout (replica la regla de Producto.PrecioEfectivo).
public class ConsultaVarianteCheckoutEf : IConsultaVarianteCheckout
{
    private readonly ApplicationDbContext _context;

    public ConsultaVarianteCheckoutEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Existe, decimal PrecioUnitario)> ObtenerAsync(int productoVarianteId, CancellationToken ct)
    {
        var datos = await _context.ProductoVariantes
            .Where(v => v.Id == productoVarianteId)
            .Select(v => new { v.Producto!.Precio, v.Producto.EnOferta, v.Producto.PrecioOferta })
            .FirstOrDefaultAsync(ct);

        if (datos is null)
        {
            return (false, 0m);
        }

        var precioEfectivo = datos.EnOferta && datos.PrecioOferta.HasValue ? datos.PrecioOferta.Value : datos.Precio;
        return (true, precioEfectivo);
    }
}
