using VoltajeModa.HttpServices.Dtos;

namespace VoltajeModa.HttpServices;

public interface IProductoApiClient
{
    Task<CatalogoPaginadoDto?> ObtenerCatalogoAsync(int pagina = 1, int tamanoPagina = 50, int? categoriaId = null, string? busqueda = null, decimal? precioMin = null, decimal? precioMax = null);
    Task<ProductoDetalleDto?> ObtenerProductoPorIdAsync(int id);
    Task<List<CategoriaDto>> ObtenerCategoriasAsync();
    Task<List<BannerDto>> ObtenerBannersAsync();
    Task<bool> CrearCategoriaAsync(string nombre);
    Task<bool> CrearProductoAsync(CrearProductoDto dto);
    Task<bool> ActualizarProductoAsync(int id, CrearProductoDto dto);
    Task<bool> CambiarEstadoProductoAsync(int id);
    Task<List<VarianteDto>> ObtenerVariantesAsync(int productoId);
    Task<bool> CrearVarianteAsync(CrearVarianteApiDto dto);
    Task<bool> ActualizarVarianteAsync(int varianteId, CrearVarianteApiDto dto);
    Task<bool> EliminarVarianteAsync(int varianteId);
    Task<bool> CambiarEstadoVarianteAsync(int varianteId);
    Task<bool> AjustarInventarioAsync(int varianteId, int cantidad, string motivo);
    Task<bool> AgregarImagenAsync(AgregarImagenApiDto dto);
    Task<bool> EliminarImagenAsync(int imagenId);
    Task<CarritoDto?> ObtenerCarritoAsync();
    Task<bool> AgregarAlCarritoAsync(int varianteId, int cantidad);
    Task<bool> ActualizarCantidadCarritoAsync(int itemId, int cantidad);
    Task<bool> EliminarItemCarritoAsync(int itemId);
    Task<List<DireccionDto>> ObtenerDireccionesAsync();
    Task<bool> CrearDireccionAsync(CrearDireccionApiDto dto);
    Task<bool> EliminarDireccionAsync(int direccionId);
    Task<PerfilDto?> ObtenerPerfilAsync();
    Task<bool> ActualizarPerfilAsync(ActualizarPerfilApiDto dto);
    Task<bool> CambiarPasswordAsync(CambiarPasswordApiDto dto);
    Task<List<MetodoPagoDto>> ObtenerMetodosPagoAsync();
    Task<PedidoCreadoDto?> CrearPedidoAsync(RealizarPedidoApiDto dto);
    Task<List<PedidoResumenDto>> ObtenerMisPedidosAsync();
    Task<PedidoClienteDto?> ObtenerMiPedidoAsync(int pedidoId);
    Task<DashboardAdminDto?> ObtenerDashboardAdminAsync();
    Task<EstadisticasAdminDto?> ObtenerEstadisticasAdminAsync();
    Task<ResumenInventarioDto?> ObtenerResumenInventarioAsync();
    Task<List<HistorialInventarioDto>> ObtenerHistorialInventarioAsync();
    Task<List<PedidoAdminResumenDto>> ObtenerPedidosAdminAsync();
    Task<PedidoAdminDto?> ObtenerPedidoAdminAsync(int pedidoId);
    Task<bool> CambiarEstadoPedidoAsync(int pedidoId, string estado);
    Task<List<UsuarioAdminDto>> ObtenerClientesAdminAsync();
    Task<List<UsuarioAdminDto>> ObtenerEmpleadosAdminAsync();
    Task<bool> CambiarEstadoClienteAsync(int usuarioId);
    Task<bool> CambiarEstadoEmpleadoAsync(int usuarioId);
    Task<bool> CambiarRolAsync(int usuarioId, int nuevoRolId);
    Task<List<CuponAdminDto>> ObtenerCuponesAdminAsync();
    Task<bool> CrearCuponAsync(CrearCuponApiDto dto);
    Task<bool> RegistrarClienteAsync(RegistroClienteDto dto);
    Task<bool> IniciarSesionAsync(string correo, string password);
}
