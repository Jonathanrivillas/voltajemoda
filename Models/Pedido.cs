using Microsoft.EntityFrameworkCore;

namespace VoltajeModa.Models;

public enum EstadoPedido
{
    Pendiente,
    Enviado,
    Entregado,
    Cancelado
}

public class Pedido
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

    [Precision(18, 2)]
    public decimal Total { get; set; }

    public int DireccionId { get; set; }
    public Direccion? Direccion { get; set; }

    public List<PedidoItem> Items { get; set; } = new();
}
