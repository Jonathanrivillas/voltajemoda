using System.Security.Claims;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;

namespace VoltajeModa.Presentacion.Servicios;

// Resuelve el Titular (usuario autenticado o invitado) para los controladores de Carrito/Pedidos/
// Direcciones, sin que la capa de aplicación necesite conocer HttpContext. La identidad de invitado
// se sostiene con una cookie propia porque ProtectedLocalStorage (usado en Blazor Server) no existe
// en un request HTTP puro de API.
public class ContextoIdentidadHttp
{
    public const string NombreCookieAnonimo = "voltajemoda.carrito.anonimo";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextoIdentidadHttp(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext HttpContext => _httpContextAccessor.HttpContext!;

    public string? UsuarioIdActual => HttpContext.User.Identity?.IsAuthenticated == true
        ? HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
        : null;

    public Titular ObtenerOAsignarTitular()
    {
        var usuarioId = UsuarioIdActual;
        if (usuarioId is not null)
        {
            return Titular.DeUsuario(usuarioId);
        }

        var anonimoId = HttpContext.Request.Cookies[NombreCookieAnonimo];
        if (string.IsNullOrEmpty(anonimoId))
        {
            anonimoId = Guid.NewGuid().ToString();
            HttpContext.Response.Cookies.Append(NombreCookieAnonimo, anonimoId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api",
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(180)
            });
        }

        return Titular.DeInvitado(anonimoId);
    }

    public string? ObtenerAnonimoIdDeCookie() => HttpContext.Request.Cookies[NombreCookieAnonimo];

    public void EliminarCookieAnonimo() => HttpContext.Response.Cookies.Delete(NombreCookieAnonimo, new CookieOptions { Path = "/api" });
}
