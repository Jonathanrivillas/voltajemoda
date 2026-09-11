using Microsoft.EntityFrameworkCore;

namespace VoltajeModa.Models;

public class PedidoItem
{
    public int Id { get; set; }
    public int Cantidad { get; set; }

    [Precision(18, 2)]
    public decimal PrecioUnitario { get; set; }

    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public int ProductoVarianteId { get; set; }
    public ProductoVariante? ProductoVariante { get; set; }
}
