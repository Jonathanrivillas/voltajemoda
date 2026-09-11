using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using VoltajeModa.Data;
using VoltajeModa.Models;

namespace VoltajeModa.Services;

public class CarritoService(ApplicationDbContext db, ProtectedLocalStorage almacenLocal)
{
    private const string ClaveAnonimo = "voltajemoda-carrito-anonimo";

    public async Task<Carrito> ObtenerOCrearCarritoAsync(string? usuarioId)
    {
        if (usuarioId is not null)
        {
            await FusionarCarritoAnonimoAsync(usuarioId);

            var carritoUsuario = await db.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
            if (carritoUsuario is not null)
            {
                return carritoUsuario;
            }

            var nuevoCarrito = new Carrito { UsuarioId = usuarioId };
            db.Carritos.Add(nuevoCarrito);
            await db.SaveChangesAsync();
            return nuevoCarrito;
        }

        var anonimoId = await ObtenerOCrearIdAnonimoAsync();
        var carritoAnonimo = await db.Carritos.FirstOrDefaultAsync(c => c.AnonimoId == anonimoId);
        if (carritoAnonimo is not null)
        {
            return carritoAnonimo;
        }

        var nuevoCarritoAnonimo = new Carrito { AnonimoId = anonimoId };
        db.Carritos.Add(nuevoCarritoAnonimo);
        await db.SaveChangesAsync();
        return nuevoCarritoAnonimo;
    }

    public async Task<string> ObtenerOCrearIdAnonimoAsync()
    {
        var resultado = await almacenLocal.GetAsync<string>(ClaveAnonimo);
        if (resultado.Success && !string.IsNullOrEmpty(resultado.Value))
        {
            return resultado.Value;
        }

        var nuevoId = Guid.NewGuid().ToString();
        await almacenLocal.SetAsync(ClaveAnonimo, nuevoId);
        return nuevoId;
    }

    private async Task FusionarCarritoAnonimoAsync(string usuarioId)
    {
        var resultado = await almacenLocal.GetAsync<string>(ClaveAnonimo);
        if (!resultado.Success || string.IsNullOrEmpty(resultado.Value))
        {
            return;
        }

        var anonimoId = resultado.Value;

        // Reasignar direcciones y pedidos hechos como invitado a la cuenta con la que se acaba de iniciar sesión.
        var direccionesAnonimas = await db.Direcciones.Where(d => d.AnonimoId == anonimoId).ToListAsync();
        foreach (var direccion in direccionesAnonimas)
        {
            direccion.UsuarioId = usuarioId;
            direccion.AnonimoId = null;
        }

        var pedidosAnonimos = await db.Pedidos.Where(p => p.AnonimoId == anonimoId).ToListAsync();
        foreach (var pedido in pedidosAnonimos)
        {
            pedido.UsuarioId = usuarioId;
            pedido.AnonimoId = null;
        }

        var carritoAnonimo = await db.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.AnonimoId == anonimoId);

        if (carritoAnonimo is null)
        {
            await db.SaveChangesAsync();
            await almacenLocal.DeleteAsync(ClaveAnonimo);
            return;
        }

        var carritoUsuario = await db.Carritos
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carritoUsuario is null)
        {
            carritoAnonimo.UsuarioId = usuarioId;
            carritoAnonimo.AnonimoId = null;
        }
        else
        {
            foreach (var item in carritoAnonimo.Items)
            {
                var existente = carritoUsuario.Items
                    .FirstOrDefault(i => i.ProductoVarianteId == item.ProductoVarianteId);

                if (existente is not null)
                {
                    existente.Cantidad += item.Cantidad;
                }
                else
                {
                    carritoUsuario.Items.Add(new CarritoItem
                    {
                        ProductoVarianteId = item.ProductoVarianteId,
                        Cantidad = item.Cantidad
                    });
                }
            }

            db.Carritos.Remove(carritoAnonimo);
        }

        await db.SaveChangesAsync();
        await almacenLocal.DeleteAsync(ClaveAnonimo);
    }
}
