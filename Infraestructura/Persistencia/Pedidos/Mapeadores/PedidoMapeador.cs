using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Pedidos.Mapeadores;

public static class PedidoMapeador
{
    public static Pedido ADominio(EfModels.Pedido entidad)
    {
        var titular = entidad.UsuarioId is not null
            ? Titular.DeUsuario(entidad.UsuarioId)
            : Titular.DeInvitado(entidad.AnonimoId!);

        var items = entidad.Items.Select(i => new PedidoItem(i.Id, i.ProductoVarianteId, i.Cantidad, i.PrecioUnitario));

        return Pedido.Reconstituir(entidad.Id, titular, entidad.Fecha, (EstadoPedido)(int)entidad.Estado, entidad.Total, entidad.DireccionId, items);
    }

    public static EfModels.Pedido AEntidadNueva(Pedido pedido) => new()
    {
        UsuarioId = pedido.Titular.UsuarioId,
        AnonimoId = pedido.Titular.AnonimoId,
        Fecha = pedido.Fecha,
        Estado = (EfModels.EstadoPedido)(int)pedido.Estado,
        Total = pedido.Total,
        DireccionId = pedido.DireccionId,
        Items = pedido.Items
            .Select(i => new EfModels.PedidoItem { ProductoVarianteId = i.ProductoVarianteId, Cantidad = i.Cantidad, PrecioUnitario = i.PrecioUnitario })
            .ToList()
    };

    // Los casos de uso actuales solo cambian Estado (CambiarEstadoPedidoCasoDeUso) o el titular
    // (fusión de invitado a usuario): Fecha, DireccionId e Items no se sincronizan porque ningún
    // caso de uso edita las líneas de un pedido ya creado.
    public static void VolcarEnEntidad(Pedido pedido, EfModels.Pedido entidad)
    {
        entidad.UsuarioId = pedido.Titular.UsuarioId;
        entidad.AnonimoId = pedido.Titular.AnonimoId;
        entidad.Estado = (EfModels.EstadoPedido)(int)pedido.Estado;
        entidad.Total = pedido.Total;
    }
}
