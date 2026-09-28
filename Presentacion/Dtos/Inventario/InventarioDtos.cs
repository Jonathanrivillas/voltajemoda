using VoltajeModa.Core.Aplicacion.Inventario.Dtos;
using VoltajeModa.Core.Dominio.Inventario;

namespace VoltajeModa.Presentacion.Dtos.Inventario;

public record VarianteInventarioDto(
    int ProductoVarianteId, int ProductoId, string ProductoNombre, string ImagenUrl,
    string Talla, string Color, int Stock, string Estado, int CategoriaId, string CategoriaNombre)
{
    public static VarianteInventarioDto DesdeLectura(VarianteInventarioLectura l) => new(
        l.ProductoVarianteId, l.ProductoId, l.ProductoNombre, l.ImagenUrl, l.Talla, l.Color, l.Stock,
        PoliticaStock.EstadoDe(l.Stock).ToString(), l.CategoriaId, l.CategoriaNombre);
}

public record MovimientoStockDto(
    int Id, int ProductoVarianteId, string Tipo, int Cantidad, string Motivo,
    DateTime Fecha, string? UsuarioId, int StockResultante)
{
    public static MovimientoStockDto DesdeDominio(MovimientoStock m) => new(
        m.Id, m.ProductoVarianteId, m.Tipo.ToString(), m.Cantidad, m.Motivo, m.Fecha, m.UsuarioId, m.StockResultante);
}

public record RegistrarMovimientoRequest(int ProductoVarianteId, TipoMovimiento Tipo, int Cantidad, string Motivo);
