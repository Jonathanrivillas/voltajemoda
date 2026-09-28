using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos.Excepciones;

namespace VoltajeModa.Core.Dominio.Pedidos;

public class Pedido
{
    private static readonly Dictionary<EstadoPedido, EstadoPedido[]> TransicionesValidas = new()
    {
        [EstadoPedido.Pendiente] = new[] { EstadoPedido.Enviado, EstadoPedido.Cancelado },
        [EstadoPedido.Enviado] = new[] { EstadoPedido.Entregado, EstadoPedido.Cancelado },
        [EstadoPedido.Entregado] = Array.Empty<EstadoPedido>(),
        [EstadoPedido.Cancelado] = Array.Empty<EstadoPedido>()
    };

    private readonly List<PedidoItem> _items = new();

    private Pedido(int id, Titular titular, DateTime fecha, EstadoPedido estado, decimal total, int direccionId)
    {
        Id = id;
        Titular = titular;
        Fecha = fecha;
        Estado = estado;
        Total = total;
        DireccionId = direccionId;
    }

    public int Id { get; private set; }
    public Titular Titular { get; private set; }
    public DateTime Fecha { get; private set; }
    public EstadoPedido Estado { get; private set; }
    public decimal Total { get; private set; }
    public int DireccionId { get; private set; }
    public IReadOnlyList<PedidoItem> Items => _items.AsReadOnly();

    public static Pedido CrearDesdeCarrito(
        Titular titular, int direccionId, IEnumerable<(int ProductoVarianteId, int Cantidad, decimal PrecioUnitario)> lineas)
    {
        var lineasList = lineas.ToList();
        if (lineasList.Count == 0)
        {
            throw new PedidoVacioException();
        }

        var pedido = new Pedido(0, titular, DateTime.UtcNow, EstadoPedido.Pendiente, 0m, direccionId);
        foreach (var linea in lineasList)
        {
            pedido._items.Add(new PedidoItem(0, linea.ProductoVarianteId, linea.Cantidad, linea.PrecioUnitario));
        }

        pedido.Total = pedido._items.Sum(i => i.CalcularSubtotal());
        return pedido;
    }

    internal static Pedido Reconstituir(
        int id, Titular titular, DateTime fecha, EstadoPedido estado, decimal total, int direccionId, IEnumerable<PedidoItem> items)
    {
        var pedido = new Pedido(id, titular, fecha, estado, total, direccionId);
        pedido._items.AddRange(items);
        return pedido;
    }

    public void CambiarEstado(EstadoPedido nuevoEstado)
    {
        if (!TransicionesValidas[Estado].Contains(nuevoEstado))
        {
            throw new TransicionEstadoInvalidaException(Estado, nuevoEstado);
        }

        Estado = nuevoEstado;
    }

    internal void ReasignarTitular(Titular nuevoTitular) => Titular = nuevoTitular;

    internal void AsignarId(int id) => Id = id;
}
