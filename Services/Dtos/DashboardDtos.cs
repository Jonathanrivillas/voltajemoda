using VoltajeModa.Models;

namespace VoltajeModa.Services.Dtos;

public record KpiResumenDto(
    decimal IngresosHoy,
    decimal IngresosMes,
    int PedidosHoy,
    int PedidosPendientes,
    int ProductosBajoStock,
    int TotalProductos,
    int TotalCategorias);

public record PuntoVentaDiariaDto(DateOnly Fecha, decimal Total);

public record ProductoMasVendidoDto(int ProductoId, string Nombre, int UnidadesVendidas, decimal IngresosGenerados);

public record PedidoRecienteDto(int PedidoId, DateTime Fecha, string Cliente, decimal Total, EstadoPedido Estado);

public record AlertaStockDto(int ProductoVarianteId, string ProductoNombre, string Talla, string Color, int Stock);
