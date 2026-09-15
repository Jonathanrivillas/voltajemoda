namespace VoltajeModa.Models;

public enum TipoMovimientoStock
{
    Entrada,
    Salida
}

public class MovimientoStock
{
    public int Id { get; set; }

    public int ProductoVarianteId { get; set; }
    public ProductoVariante? ProductoVariante { get; set; }

    public TipoMovimientoStock Tipo { get; set; }
    public int Cantidad { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public string? UsuarioId { get; set; }

    public int StockResultante { get; set; }
}
