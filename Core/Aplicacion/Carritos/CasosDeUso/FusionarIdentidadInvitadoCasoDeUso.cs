using VoltajeModa.Core.Aplicacion.Carritos.Interfaces;
using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Carritos;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;

namespace VoltajeModa.Core.Aplicacion.Carritos.CasosDeUso;

// Orquestador cross-BC intencional: al iniciar sesión, todo lo que el invitado tenía bajo su
// AnonimoId (direcciones, pedidos, carrito) se reasigna al UsuarioId, en un único commit. Usa los
// puertos angostos de Pedidos (IRepositorioDirecciones, IRepositorioPedidos) inyectados
// directamente, no un acceso genérico a ese bounded context.
public class FusionarIdentidadInvitadoCasoDeUso
{
    private readonly IRepositorioDirecciones _direcciones;
    private readonly IRepositorioPedidos _pedidos;
    private readonly IRepositorioCarritos _carritos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public FusionarIdentidadInvitadoCasoDeUso(
        IRepositorioDirecciones direcciones, IRepositorioPedidos pedidos, IRepositorioCarritos carritos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _direcciones = direcciones;
        _pedidos = pedidos;
        _carritos = carritos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task EjecutarAsync(string usuarioId, string anonimoId, CancellationToken ct)
    {
        var titularInvitado = Titular.DeInvitado(anonimoId);
        var titularUsuario = Titular.DeUsuario(usuarioId);

        var direcciones = await _direcciones.ListarPorTitularAsync(titularInvitado, ct);
        foreach (var direccion in direcciones)
        {
            direccion.ReasignarTitular(titularUsuario);
            await _direcciones.ActualizarAsync(direccion, ct);
        }

        var pedidos = await _pedidos.ListarPorTitularAsync(titularInvitado, ct);
        foreach (var pedido in pedidos)
        {
            pedido.ReasignarTitular(titularUsuario);
            await _pedidos.ActualizarAsync(pedido, ct);
        }

        var carritoAnonimo = await _carritos.ObtenerPorAnonimoAsync(anonimoId, ct);
        if (carritoAnonimo is not null)
        {
            var carritoUsuario = await _carritos.ObtenerPorUsuarioAsync(usuarioId, ct);
            if (carritoUsuario is not null)
            {
                carritoUsuario.FusionarCon(carritoAnonimo);
                await _carritos.ActualizarAsync(carritoUsuario, ct);
            }
            else
            {
                carritoUsuario = Carrito.Crear(titularUsuario);
                carritoUsuario.FusionarCon(carritoAnonimo);
                await _carritos.AgregarAsync(carritoUsuario, ct);
            }

            await _carritos.EliminarAsync(carritoAnonimo.Id, ct);
        }

        await _unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
