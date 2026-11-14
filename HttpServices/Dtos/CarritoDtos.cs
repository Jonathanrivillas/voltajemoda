namespace VoltajeModa.HttpServices.Dtos;

public class CarritoDto
{
    public List<CarritoItemDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class CarritoItemDto
{
    public int CarritoItemId { get; set; }
    public int VarianteId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string SKU_Variante { get; set; } = string.Empty;
    public string? Talla { get; set; }
    public string? Color { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => PrecioUnitario * Cantidad;
}

public class CrearDireccionApiDto
{
    public string Etiqueta { get; set; } = string.Empty;
    public string DireccionFisica { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
}

public class DireccionDto
{
    public int DireccionId { get; set; }
    public string Etiqueta { get; set; } = string.Empty;
    public string DireccionFisica { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class PerfilDto
{
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Telefono { get; set; }
}

public class ActualizarPerfilApiDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}

public class CambiarPasswordApiDto
{
    public string PasswordActual { get; set; } = string.Empty;
    public string NuevaPassword { get; set; } = string.Empty;
}

public class MetodoPagoDto
{
    public int MetodoPagoId { get; set; }
    public string NombreMetodo { get; set; } = string.Empty;
}

public class RealizarPedidoApiDto
{
    public string DireccionEnvio { get; set; } = string.Empty;
    public string CiudadEnvio { get; set; } = string.Empty;
    public int MetodoPagoId { get; set; }
    public int? CuponId { get; set; }
}

public class CuponValidadoDto
{
    public int CuponId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public decimal PorcentajeDescuento { get; set; }
}

public class PedidoCreadoDto
{
    public int PedidoId { get; set; }
}

public class PedidoResumenDto
{
    public int PedidoId { get; set; }
    public DateTime FechaPedido { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int CantidadArticulos { get; set; }
}

public class PedidoClienteDto : PedidoResumenDto
{
    public string DireccionEnvio { get; set; } = string.Empty;
    public string CiudadEnvio { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public List<PedidoClienteItemDto> Detalles { get; set; } = new();
}

public class PedidoClienteItemDto
{
    public string Nombre { get; set; } = string.Empty;
    public string SkuVariante { get; set; } = string.Empty;
    public string? Talla { get; set; }
    public string? Color { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
