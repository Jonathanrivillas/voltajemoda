using VoltajeModa.Core.Dominio.Inventario.Excepciones;

namespace VoltajeModa.Core.Dominio.Inventario;

public class MovimientoStock
{
    private MovimientoStock(
        int id, int productoVarianteId, TipoMovimiento tipo, int cantidad,
        string motivo, DateTime fecha, string? usuarioId, int stockResultante)
    {
        Id = id;
        ProductoVarianteId = productoVarianteId;
        Tipo = tipo;
        Cantidad = cantidad;
        Motivo = motivo;
        Fecha = fecha;
        UsuarioId = usuarioId;
        StockResultante = stockResultante;
    }

    public int Id { get; private set; }
    public int ProductoVarianteId { get; private set; }
    public TipoMovimiento Tipo { get; private set; }
    public int Cantidad { get; private set; }
    public string Motivo { get; private set; }
    public DateTime Fecha { get; private set; }
    public string? UsuarioId { get; private set; }
    public int StockResultante { get; private set; }

    public static (MovimientoStock Movimiento, int NuevoStock) Registrar(
        int productoVarianteId, TipoMovimiento tipo, int cantidad, string motivo, string? usuarioId, int stockActual)
    {
        if (cantidad <= 0)
        {
            throw new CantidadInvalidaException("La cantidad del movimiento debe ser mayor a 0.");
        }

        if (tipo == TipoMovimiento.Salida && cantidad > stockActual)
        {
            throw new StockInsuficienteException(productoVarianteId, stockActual, cantidad);
        }

        var nuevoStock = stockActual + (tipo == TipoMovimiento.Entrada ? cantidad : -cantidad);
        var movimiento = new MovimientoStock(0, productoVarianteId, tipo, cantidad, motivo ?? string.Empty, DateTime.UtcNow, usuarioId, nuevoStock);

        return (movimiento, nuevoStock);
    }

    internal static MovimientoStock Reconstituir(
        int id, int productoVarianteId, TipoMovimiento tipo, int cantidad,
        string motivo, DateTime fecha, string? usuarioId, int stockResultante)
        => new(id, productoVarianteId, tipo, cantidad, motivo, fecha, usuarioId, stockResultante);

    internal void AsignarId(int id) => Id = id;
}
