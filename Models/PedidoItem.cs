using System.ComponentModel.DataAnnotations;

namespace VoltajeModa.Models;

public class PedidoItem
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0.")]
    public decimal PrecioUnitario { get; set; }

    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public int ProductoVarianteId { get; set; }
    public ProductoVariante? ProductoVariante { get; set; }
}
