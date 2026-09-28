using VoltajeModa.Core.Dominio.Inventario;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Inventario.Mapeadores;

public static class MovimientoStockMapeador
{
    public static MovimientoStock ADominio(EfModels.MovimientoStock entidad) => MovimientoStock.Reconstituir(
        entidad.Id, entidad.ProductoVarianteId, (TipoMovimiento)(int)entidad.Tipo, entidad.Cantidad,
        entidad.Motivo, entidad.Fecha, entidad.UsuarioId, entidad.StockResultante);

    public static EfModels.MovimientoStock AEntidadNueva(MovimientoStock movimiento) => new()
    {
        ProductoVarianteId = movimiento.ProductoVarianteId,
        Tipo = (EfModels.TipoMovimientoStock)(int)movimiento.Tipo,
        Cantidad = movimiento.Cantidad,
        Motivo = movimiento.Motivo,
        Fecha = movimiento.Fecha,
        UsuarioId = movimiento.UsuarioId,
        StockResultante = movimiento.StockResultante
    };
}
