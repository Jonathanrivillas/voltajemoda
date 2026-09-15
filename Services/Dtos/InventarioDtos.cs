using VoltajeModa.Models;

namespace VoltajeModa.Services.Dtos;

public enum EstadoStockFiltro
{
    Todos,
    EnStock,
    StockBajo,
    SinStock
}

public record VarianteInventarioDto(
    int ProductoVarianteId,
    int ProductoId,
    string ProductoNombre,
    string ImagenUrl,
    string Talla,
    string Color,
    int Stock,
    int CategoriaId,
    string CategoriaNombre);

public record ResultadoMovimientoStock(bool Exito, string? Error, MovimientoStock? Movimiento);
