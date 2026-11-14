using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Logging;
using VoltajeModa.HttpServices.Dtos;
using VoltajeModa.Services;

namespace VoltajeModa.HttpServices;

public class ProductoApiClient : IProductoApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<ProductoApiClient> _logger;
    private readonly SesionUsuarioService _sesion;

    public ProductoApiClient(HttpClient http, ILogger<ProductoApiClient> logger, SesionUsuarioService sesion)
    {
        _http = http;
        _logger = logger;
        _sesion = sesion;
    }

    public async Task<CatalogoPaginadoDto?> ObtenerCatalogoAsync(
        int pagina = 1,
        int tamanoPagina = 50,
        int? categoriaId = null,
        string? busqueda = null,
        decimal? precioMin = null,
        decimal? precioMax = null)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"pagina={pagina}",
                $"tamanoPagina={tamanoPagina}"
            };

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                queryParams.Add($"categoriaId={categoriaId.Value}");
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                queryParams.Add($"busqueda={Uri.EscapeDataString(busqueda.Trim())}");
            }

            if (precioMin.HasValue)
            {
                queryParams.Add($"precioMin={precioMin.Value}");
            }

            if (precioMax.HasValue)
            {
                queryParams.Add($"precioMax={precioMax.Value}");
            }

            var url = "Productos?" + string.Join("&", queryParams);
            var resultado = await ObtenerJsonAsync<CatalogoPaginadoDto>(url);
            return resultado ?? new CatalogoPaginadoDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar el catálogo de productos desde el backend.");
            return new CatalogoPaginadoDto();
        }
    }

    public async Task<ProductoDetalleDto?> ObtenerProductoPorIdAsync(int id)
    {
        try
        {
            return await ObtenerJsonAsync<ProductoDetalleDto>($"Productos/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el producto con ID {ProductoId} desde el backend.", id);
            return null;
        }
    }

    public async Task<List<CategoriaDto>> ObtenerCategoriasAsync()
    {
        try
        {
            var categorias = await ObtenerJsonAsync<List<CategoriaDto>>("Categorias");
            return categorias ?? new List<CategoriaDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las categorías desde el backend.");
            return new List<CategoriaDto>();
        }
    }

    public async Task<List<BannerDto>> ObtenerBannersAsync()
    {
        try
        {
            return await ObtenerJsonAsync<List<BannerDto>>("Banners") ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener los banners desde el backend.");
            return new();
        }
    }

    public async Task<bool> CrearCategoriaAsync(string nombre)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("Categorias", new { Nombre = nombre });
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al crear la categoría desde el backend.");
            return false;
        }
    }

    public async Task<bool> CrearProductoAsync(CrearProductoDto dto)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("Productos", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al crear el producto desde el backend.");
            return false;
        }
    }

    public async Task<bool> ActualizarProductoAsync(int id, CrearProductoDto dto)
    {
        try
        {
            using var respuesta = await EnviarJsonAsync(HttpMethod.Put, $"Productos/{id}", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al actualizar el producto {ProductoId} desde el backend.", id);
            return false;
        }
    }

    public async Task<bool> CambiarEstadoProductoAsync(int id)
    {
        try
        {
            using var solicitud = new HttpRequestMessage(HttpMethod.Put, $"Productos/{id}/estado");
            AgregarToken(solicitud);
            using var respuesta = await EnviarSolicitudAsync(solicitud);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al cambiar el estado del producto {ProductoId}.", id);
            return false;
        }
    }

    public async Task<List<VarianteDto>> ObtenerVariantesAsync(int productoId)
    {
        try
        {
            return await ObtenerJsonAsync<List<VarianteDto>>($"Variantes/producto/{productoId}") ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar las variantes del producto {ProductoId}.", productoId);
            return new();
        }
    }

    public async Task<bool> CrearVarianteAsync(CrearVarianteApiDto dto)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("Variantes", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al crear una variante para el producto {ProductoId}.", dto.ProductoId);
            return false;
        }
    }

    public async Task<bool> ActualizarVarianteAsync(int varianteId, CrearVarianteApiDto dto)
    {
        try
        {
            using var respuesta = await EnviarJsonAsync(HttpMethod.Put, $"Variantes/{varianteId}", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al actualizar la variante {VarianteId}.", varianteId);
            return false;
        }
    }

    public async Task<bool> EliminarVarianteAsync(int varianteId)
    {
        try
        {
            using var solicitud = new HttpRequestMessage(HttpMethod.Delete, $"Variantes/{varianteId}");
            AgregarToken(solicitud);
            using var respuesta = await EnviarSolicitudAsync(solicitud);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al desactivar la variante {VarianteId}.", varianteId);
            return false;
        }
    }

    public async Task<bool> CambiarEstadoVarianteAsync(int varianteId)
    {
        try
        {
            using var solicitud = new HttpRequestMessage(HttpMethod.Put, $"Variantes/{varianteId}/estado");
            AgregarToken(solicitud);
            using var respuesta = await EnviarSolicitudAsync(solicitud);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al cambiar el estado de la variante {VarianteId}.", varianteId);
            return false;
        }
    }

    public async Task<bool> AjustarInventarioAsync(int varianteId, int cantidad, string motivo)
    {
        try
        {
            using var respuesta = await EnviarJsonAsync(HttpMethod.Post, $"Variantes/{varianteId}/inventario", new
            {
                CantidadAjuste = cantidad,
                Motivo = motivo
            });
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al ajustar el inventario de la variante {VarianteId}.", varianteId);
            return false;
        }
    }

    public async Task<bool> AgregarImagenAsync(AgregarImagenApiDto dto)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("ProductoImagenes", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al agregar una imagen al producto {ProductoId}.", dto.ProductoId);
            return false;
        }
    }

    public async Task<bool> EliminarImagenAsync(int imagenId)
    {
        try
        {
            using var solicitud = new HttpRequestMessage(HttpMethod.Delete, $"ProductoImagenes/{imagenId}");
            AgregarToken(solicitud);
            using var respuesta = await EnviarSolicitudAsync(solicitud);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al eliminar la imagen {ImagenId}.", imagenId);
            return false;
        }
    }

    public async Task<CarritoDto?> ObtenerCarritoAsync()
    {
        try
        {
            return await ObtenerJsonAsync<CarritoDto>("Carrito");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener el carrito desde el backend.");
            return null;
        }
    }

    public async Task<bool> AgregarAlCarritoAsync(int varianteId, int cantidad)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("Carrito/items", new { VarianteId = varianteId, Cantidad = cantidad });
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al agregar una variante al carrito.");
            return false;
        }
    }

    public async Task<bool> ActualizarCantidadCarritoAsync(int itemId, int cantidad)
    {
        try
        {
            using var respuesta = await EnviarJsonAsync(HttpMethod.Put, $"Carrito/items/{itemId}", new { Cantidad = cantidad });
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al actualizar el ítem {CarritoItemId}.", itemId);
            return false;
        }
    }

    public async Task<bool> EliminarItemCarritoAsync(int itemId)
    {
        try
        {
            using var solicitud = new HttpRequestMessage(HttpMethod.Delete, $"Carrito/items/{itemId}");
            AgregarToken(solicitud);
            using var respuesta = await EnviarSolicitudAsync(solicitud);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al eliminar el ítem {CarritoItemId}.", itemId);
            return false;
        }
    }

    public async Task<List<DireccionDto>> ObtenerDireccionesAsync()
    {
        try
        {
            return await ObtenerJsonAsync<List<DireccionDto>>("Direcciones") ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener las direcciones.");
            return new();
        }
    }

    public async Task<bool> CrearDireccionAsync(CrearDireccionApiDto dto)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("Direcciones", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al guardar una dirección.");
            return false;
        }
    }

    public async Task<bool> EliminarDireccionAsync(int direccionId)
    {
        try
        {
            using var solicitud = new HttpRequestMessage(HttpMethod.Delete, $"Direcciones/{direccionId}");
            AgregarToken(solicitud);
            using var respuesta = await EnviarSolicitudAsync(solicitud);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al eliminar la dirección {DireccionId}.", direccionId);
            return false;
        }
    }

    public async Task<PerfilDto?> ObtenerPerfilAsync()
    {
        try
        {
            return await ObtenerJsonAsync<PerfilDto>("Perfil");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al consultar el perfil.");
            return null;
        }
    }

    public async Task<bool> ActualizarPerfilAsync(ActualizarPerfilApiDto dto)
    {
        try
        {
            using var respuesta = await EnviarJsonAsync(HttpMethod.Put, "Perfil", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al actualizar el perfil.");
            return false;
        }
    }

    public async Task<bool> CambiarPasswordAsync(CambiarPasswordApiDto dto)
    {
        try
        {
            using var respuesta = await EnviarJsonAsync(HttpMethod.Put, "Perfil/password", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al cambiar la contraseña.");
            return false;
        }
    }

    public async Task<List<MetodoPagoDto>> ObtenerMetodosPagoAsync()
    {
        try
        {
            return await ObtenerJsonAsync<List<MetodoPagoDto>>("Pedidos/metodos-pago") ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al consultar los métodos de pago.");
            return new();
        }
    }

    public async Task<CuponValidadoDto?> ValidarCuponAsync(string codigo)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("Cupones/validar", new { Codigo = codigo });
            if (!respuesta.IsSuccessStatusCode)
            {
                _logger.LogWarning("No se pudo validar el cupón. Código HTTP: {StatusCode}.", respuesta.StatusCode);
                return null;
            }

            return await respuesta.Content.ReadFromJsonAsync<CuponValidadoDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al validar el cupón.");
            return null;
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "No se pudo leer la respuesta de validación del cupón.");
            return null;
        }
    }

    public async Task<PedidoCreadoDto?> CrearPedidoAsync(RealizarPedidoApiDto dto)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("Pedidos", dto);
            if (!respuesta.IsSuccessStatusCode)
            {
                return null;
            }

            return await respuesta.Content.ReadFromJsonAsync<PedidoCreadoDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al procesar el pedido.");
            return null;
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "No se pudo leer la respuesta de checkout.");
            Console.Error.WriteLine($"[ProductoApiClient] Error en Pedidos: {ex.Message}");
            return null;
        }
    }

    public async Task<List<PedidoResumenDto>> ObtenerMisPedidosAsync()
    {
        try
        {
            return await ObtenerJsonAsync<List<PedidoResumenDto>>("Pedidos/mis-pedidos") ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al consultar los pedidos del cliente.");
            return new();
        }
    }

    public async Task<PedidoClienteDto?> ObtenerMiPedidoAsync(int pedidoId)
    {
        try
        {
            return await ObtenerJsonAsync<PedidoClienteDto>($"Pedidos/mis-pedidos/{pedidoId}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al consultar el pedido {PedidoId}.", pedidoId);
            return null;
        }
    }

    public async Task<DashboardAdminDto?> ObtenerDashboardAdminAsync()
    {
        try
        {
            return await ObtenerJsonAsync<DashboardAdminDto>("Dashboard/resumen");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener el resumen administrativo.");
            return null;
        }
    }

    public async Task<EstadisticasAdminDto?> ObtenerEstadisticasAdminAsync()
    {
        try
        {
            return await ObtenerJsonAsync<EstadisticasAdminDto>("Estadisticas/resumen");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener las estadísticas administrativas.");
            return null;
        }
    }

    public async Task<ResumenInventarioDto?> ObtenerResumenInventarioAsync()
    {
        try
        {
            return await ObtenerJsonAsync<ResumenInventarioDto>("Inventario/resumen");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener el resumen del inventario.");
            return null;
        }
    }

    public async Task<List<HistorialInventarioDto>> ObtenerHistorialInventarioAsync()
    {
        try
        {
            return await ObtenerJsonAsync<List<HistorialInventarioDto>>("AuditoriaInventario") ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener el historial del inventario.");
            return new();
        }
    }

    public async Task<List<PedidoAdminResumenDto>> ObtenerPedidosAdminAsync()
    {
        try
        {
            return await ObtenerJsonAsync<List<PedidoAdminResumenDto>>("AdminPedidos") ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener los pedidos administrativos.");
            return new();
        }
    }

    public async Task<PedidoAdminDto?> ObtenerPedidoAdminAsync(int pedidoId)
    {
        try
        {
            return await ObtenerJsonAsync<PedidoAdminDto>($"AdminPedidos/{pedidoId}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener el pedido {PedidoId}.", pedidoId);
            return null;
        }
    }

    public async Task<bool> CambiarEstadoPedidoAsync(int pedidoId, string estado)
    {
        try
        {
            using var respuesta = await EnviarJsonAsync(HttpMethod.Put, $"AdminPedidos/{pedidoId}/estado", new { Estado = estado });
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al cambiar el estado del pedido {PedidoId}.", pedidoId);
            return false;
        }
    }

    public async Task<List<UsuarioAdminDto>> ObtenerClientesAdminAsync()
    {
        try
        {
            return await ObtenerJsonAsync<List<UsuarioAdminDto>>("Usuarios/clientes") ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de clientes.");
            return new();
        }
    }

    public async Task<List<UsuarioAdminDto>> ObtenerEmpleadosAdminAsync()
    {
        try
        {
            return await ObtenerJsonAsync<List<UsuarioAdminDto>>("Usuarios/empleados") ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de empleados.");
            return new();
        }
    }

    public async Task<bool> CambiarEstadoClienteAsync(int usuarioId)
    {
        try
        {
            using var solicitud = new HttpRequestMessage(HttpMethod.Put, $"Usuarios/clientes/{usuarioId}/estado");
            AgregarToken(solicitud);
            using var respuesta = await EnviarSolicitudAsync(solicitud);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al cambiar el estado del cliente {UsuarioId}.", usuarioId);
            return false;
        }
    }

    public async Task<bool> CambiarEstadoEmpleadoAsync(int usuarioId)
    {
        try
        {
            using var solicitud = new HttpRequestMessage(HttpMethod.Put, $"Usuarios/empleados/{usuarioId}/estado");
            AgregarToken(solicitud);
            using var respuesta = await EnviarSolicitudAsync(solicitud);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al cambiar el estado del empleado {UsuarioId}.", usuarioId);
            return false;
        }
    }

    public async Task<bool> CambiarRolAsync(int usuarioId, int nuevoRolId)
    {
        try
        {
            using var respuesta = await EnviarJsonAsync(
                HttpMethod.Put,
                $"Usuarios/{usuarioId}/rol",
                new { NuevoRolId = nuevoRolId });
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al cambiar el rol del usuario {UsuarioId}.", usuarioId);
            return false;
        }
    }

    public async Task<List<CuponAdminDto>> ObtenerCuponesAdminAsync()
    {
        try
        {
            return await ObtenerJsonAsync<List<CuponAdminDto>>("AdminCupones") ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al obtener los cupones.");
            return new();
        }
    }

    public async Task<bool> CrearCuponAsync(CrearCuponApiDto dto)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("AdminCupones", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al crear el cupón {Codigo}.", dto.Codigo);
            return false;
        }
    }

    public async Task<bool> RegistrarClienteAsync(RegistroClienteDto dto)
    {
        try
        {
            using var respuesta = await EnviarPostAutenticadoAsync("Auth/registro", dto);
            return respuesta.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al registrar el cliente desde el backend.");
            return false;
        }
    }

    public async Task<bool> IniciarSesionAsync(string correo, string password)
    {
        await _sesion.CerrarSesionAsync();

        try
        {
            using var solicitud = new HttpRequestMessage(HttpMethod.Post, "Auth/login")
            {
                Content = JsonContent.Create(new
                {
                    CorreoElectronico = correo,
                    Password = password
                })
            };
            using var respuesta = await EnviarSolicitudAsync(solicitud);

            if (respuesta.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            var resultado = await respuesta.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (string.IsNullOrWhiteSpace(resultado?.Token) || string.IsNullOrWhiteSpace(resultado.Rol))
            {
                _logger.LogError("La respuesta de inicio de sesión no contiene el token o el rol.");
                return false;
            }

            await _sesion.IniciarSesionAsync(resultado.Token, correo, resultado.Rol);
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al iniciar sesión con el backend.");
            Console.Error.WriteLine($"[ProductoApiClient] Error en Auth/login: {ex.Message}");
            return false;
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "No se pudo leer la respuesta de inicio de sesión del backend.");
            Console.Error.WriteLine($"[ProductoApiClient] Error en Auth/login: {ex.Message}");
            return false;
        }
    }

    private async Task<HttpResponseMessage> EnviarPostAutenticadoAsync<T>(string url, T contenido)
    {
        return await EnviarJsonAsync(HttpMethod.Post, url, contenido);
    }

    private async Task<HttpResponseMessage> EnviarJsonAsync<T>(HttpMethod metodo, string url, T contenido)
    {
        using var solicitud = new HttpRequestMessage(metodo, url)
        {
            Content = JsonContent.Create(contenido)
        };

        AgregarToken(solicitud);

        return await EnviarSolicitudAsync(solicitud);
    }

    private async Task<T?> ObtenerJsonAsync<T>(string url)
    {
        using var solicitud = new HttpRequestMessage(HttpMethod.Get, url);
        AgregarToken(solicitud);
        using var respuesta = await EnviarSolicitudAsync(solicitud);
        respuesta.EnsureSuccessStatusCode();
        try
        {
            return await respuesta.Content.ReadFromJsonAsync<T>();
        }
        catch (System.Text.Json.JsonException ex)
        {
            Console.Error.WriteLine($"[ProductoApiClient] Error en {url}: {ex.Message}");
            throw;
        }
    }

    private async Task<HttpResponseMessage> EnviarSolicitudAsync(HttpRequestMessage solicitud)
    {
        var endpoint = solicitud.RequestUri?.ToString() ?? "endpoint desconocido";
        try
        {
            var respuesta = await _http.SendAsync(solicitud);
            if (!respuesta.IsSuccessStatusCode)
            {
                var detalle = await respuesta.Content.ReadAsStringAsync();
                var mensaje = $"[ProductoApiClient] Error en {endpoint}: HTTP {(int)respuesta.StatusCode} ({respuesta.StatusCode}). {detalle}";
                Console.Error.WriteLine(mensaje);
                _logger.LogError("{ApiError}", mensaje);
            }

            return respuesta;
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"[ProductoApiClient] Error en {endpoint}: {ex.Message}");
            throw;
        }
        catch (TaskCanceledException ex)
        {
            Console.Error.WriteLine($"[ProductoApiClient] Timeout con el backend en {endpoint}: {ex.Message}");
            _logger.LogWarning(ex, "Timeout con el backend al consultar {Endpoint}.", endpoint);
            return new HttpResponseMessage(HttpStatusCode.GatewayTimeout)
            {
                RequestMessage = solicitud,
                Content = JsonContent.Create(new { mensaje = "El servicio está tardando en responder. Intenta de nuevo en unos momentos." })
            };
        }
        catch (TimeoutException ex)
        {
            Console.Error.WriteLine($"[ProductoApiClient] Timeout con el backend en {endpoint}: {ex.Message}");
            _logger.LogWarning(ex, "Timeout con el backend al consultar {Endpoint}.", endpoint);
            return new HttpResponseMessage(HttpStatusCode.GatewayTimeout)
            {
                RequestMessage = solicitud,
                Content = JsonContent.Create(new { mensaje = "El servicio está tardando en responder. Intenta de nuevo en unos momentos." })
            };
        }
    }

    private void AgregarToken(HttpRequestMessage solicitud)
    {
        if (!string.IsNullOrWhiteSpace(_sesion.Token))
        {
            solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _sesion.Token);
        }
    }
}
