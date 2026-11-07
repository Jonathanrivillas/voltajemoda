namespace VoltajeModa.HttpServices.Dtos;

public class CrearVarianteApiDto
{
    public int ProductoId { get; set; }
    public string SKU_Variante { get; set; } = string.Empty;
    public string? Talla { get; set; }
    public string? Color { get; set; }
}

public class AgregarImagenApiDto
{
    public int ProductoId { get; set; }
    public string UrlImagen { get; set; } = string.Empty;
    public int Orden { get; set; }
}

public class DashboardAdminDto
{
    public int TotalProductos { get; set; }
    public int ProductosActivos { get; set; }
    public int TotalClientes { get; set; }
    public int PedidosPendientes { get; set; }
    public int VentasHoy { get; set; }
    public decimal IngresosMes { get; set; }
    public int StockBajo { get; set; }
    public List<PedidoDashboardDto> UltimosPedidos { get; set; } = new();
}

public class PedidoDashboardDto
{
    public int PedidoId { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime? FechaPedido { get; set; }
}

public class PedidoAdminResumenDto
{
    public int PedidoId { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string CiudadEnvio { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime? FechaPedido { get; set; }
}

public class PedidoAdminDto : PedidoAdminResumenDto
{
    public string NombreCliente { get; set; } = string.Empty;
    public string CorreoCliente { get; set; } = string.Empty;
    public string DireccionEnvio { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = string.Empty;
    public List<PedidoAdminItemDto> Detalles { get; set; } = new();
}

public class PedidoAdminItemDto
{
    public string NombreProducto { get; set; } = string.Empty;
    public string SKU_Variante { get; set; } = string.Empty;
    public string? Talla { get; set; }
    public string? Color { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class HistorialInventarioDto
{
    public int HistorialId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string SKU_Variante { get; set; } = string.Empty;
    public string NombreEmpleado { get; set; } = string.Empty;
    public int CantidadAjuste { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime FechaAjuste { get; set; }
}

public class ResumenInventarioDto
{
    public int TotalVariantes { get; set; }
    public int TotalStock { get; set; }
    public int VariantesSinStock { get; set; }
    public List<AlertaInventarioDto> StockBajo { get; set; } = new();
}

public class AlertaInventarioDto
{
    public int VarianteId { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string SkuVariante { get; set; } = string.Empty;
    public string? Talla { get; set; }
    public string? Color { get; set; }
    public int Stock { get; set; }
}

public class UsuarioAdminDto
{
    public int UsuarioId { get; set; }
    public int RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Cargo { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class EstadisticasAdminDto
{
    public int TotalVentasMes { get; set; }
    public decimal IngresosMes { get; set; }
    public decimal PromedioPedido { get; set; }
    public int ClientesActivos { get; set; }
    public List<ProductoMasVendidoAdminDto> ProductosMasVendidos { get; set; } = new();
    public List<VentaDiariaAdminDto> VentasPorDia { get; set; } = new();
}

public class ProductoMasVendidoAdminDto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int UnidadesVendidas { get; set; }
}

public class VentaDiariaAdminDto
{
    public DateTime Fecha { get; set; }
    public int TotalVentas { get; set; }
    public decimal Ingresos { get; set; }
}

public class CuponAdminDto
{
    public int CuponId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public decimal Descuento { get; set; }
    public DateTime ValidoDesde { get; set; }
    public DateTime ValidoHasta { get; set; }
    public bool Activo { get; set; }
}

public class CrearCuponApiDto
{
    public string Codigo { get; set; } = string.Empty;
    public decimal Descuento { get; set; }
    public DateTime ValidoDesde { get; set; }
    public DateTime ValidoHasta { get; set; }
}
