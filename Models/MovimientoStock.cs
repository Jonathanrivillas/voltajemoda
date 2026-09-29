using System.ComponentModel.DataAnnotations;

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

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad del movimiento debe ser al menos 1.")]
    public int Cantidad { get; set; }

    [Required(ErrorMessage = "El motivo del movimiento es obligatorio.")]
    [StringLength(200, ErrorMessage = "El motivo no puede superar los 200 caracteres.")]
    public string Motivo { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public string? UsuarioId { get; set; }

    public int StockResultante { get; set; }
}
