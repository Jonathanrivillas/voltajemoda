using VoltajeModa.Core.Dominio.Pedidos;

namespace VoltajeModa.Presentacion.Dtos.Pedidos;

public record PedidoItemDto(int ProductoVarianteId, int Cantidad, decimal PrecioUnitario, decimal Subtotal)
{
    public static PedidoItemDto DesdeDominio(PedidoItem i) => new(i.ProductoVarianteId, i.Cantidad, i.PrecioUnitario, i.CalcularSubtotal());
}

public record PedidoResumenDto(int Id, DateTime Fecha, string Estado, decimal Total)
{
    public static PedidoResumenDto DesdeDominio(Pedido p) => new(p.Id, p.Fecha, p.Estado.ToString(), p.Total);
}

public record PedidoDetalleDto(int Id, DateTime Fecha, string Estado, decimal Total, int DireccionId, IReadOnlyList<PedidoItemDto> Items)
{
    public static PedidoDetalleDto DesdeDominio(Pedido p) => new(
        p.Id, p.Fecha, p.Estado.ToString(), p.Total, p.DireccionId, p.Items.Select(PedidoItemDto.DesdeDominio).ToList());
}

public record CrearPedidoRequest(int DireccionId);

public record CambiarEstadoRequest(EstadoPedido NuevoEstado);
