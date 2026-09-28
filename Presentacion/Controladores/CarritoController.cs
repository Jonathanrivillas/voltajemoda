using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltajeModa.Core.Aplicacion.Carritos.CasosDeUso;
using VoltajeModa.Presentacion.Dtos.Carritos;
using VoltajeModa.Presentacion.Servicios;

namespace VoltajeModa.Presentacion.Controladores;

[ApiController]
[Route("api/carrito")]
public class CarritoController : ControllerBase
{
    private readonly ObtenerCarritoCasoDeUso _obtener;
    private readonly AgregarItemCarritoCasoDeUso _agregarItem;
    private readonly ActualizarCantidadItemCasoDeUso _actualizarCantidad;
    private readonly QuitarItemCarritoCasoDeUso _quitarItem;
    private readonly VaciarCarritoCasoDeUso _vaciar;
    private readonly FusionarIdentidadInvitadoCasoDeUso _fusionar;
    private readonly ContextoIdentidadHttp _identidad;

    public CarritoController(
        ObtenerCarritoCasoDeUso obtener, AgregarItemCarritoCasoDeUso agregarItem,
        ActualizarCantidadItemCasoDeUso actualizarCantidad, QuitarItemCarritoCasoDeUso quitarItem,
        VaciarCarritoCasoDeUso vaciar, FusionarIdentidadInvitadoCasoDeUso fusionar, ContextoIdentidadHttp identidad)
    {
        _obtener = obtener;
        _agregarItem = agregarItem;
        _actualizarCantidad = actualizarCantidad;
        _quitarItem = quitarItem;
        _vaciar = vaciar;
        _fusionar = fusionar;
        _identidad = identidad;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<CarritoDto>> Obtener(CancellationToken ct)
    {
        var carrito = await _obtener.EjecutarAsync(_identidad.ObtenerOAsignarTitular(), ct);
        return Ok(CarritoDto.DesdeDominio(carrito));
    }

    [HttpPost("items")]
    [AllowAnonymous]
    public async Task<ActionResult<CarritoDto>> AgregarItem([FromBody] AgregarItemRequest request, CancellationToken ct)
    {
        var carrito = await _agregarItem.EjecutarAsync(_identidad.ObtenerOAsignarTitular(), request.ProductoVarianteId, request.Cantidad, ct);
        return Ok(CarritoDto.DesdeDominio(carrito));
    }

    [HttpPut("items/{productoVarianteId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CarritoDto>> ActualizarCantidad(int productoVarianteId, [FromBody] ActualizarCantidadRequest request, CancellationToken ct)
    {
        var carrito = await _actualizarCantidad.EjecutarAsync(_identidad.ObtenerOAsignarTitular(), productoVarianteId, request.Cantidad, ct);
        return Ok(CarritoDto.DesdeDominio(carrito));
    }

    [HttpDelete("items/{productoVarianteId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> QuitarItem(int productoVarianteId, CancellationToken ct)
    {
        await _quitarItem.EjecutarAsync(_identidad.ObtenerOAsignarTitular(), productoVarianteId, ct);
        return NoContent();
    }

    [HttpDelete]
    [AllowAnonymous]
    public async Task<IActionResult> Vaciar(CancellationToken ct)
    {
        await _vaciar.EjecutarAsync(_identidad.ObtenerOAsignarTitular(), ct);
        return NoContent();
    }

    // Llamado por el cliente justo después del login: no hay un hook limpio de OnSignIn accesible
    // desde fuera del pipeline de Identity Razor Components para disparar esto automáticamente.
    [HttpPost("fusionar")]
    [Authorize]
    public async Task<IActionResult> Fusionar(CancellationToken ct)
    {
        var anonimoId = _identidad.ObtenerAnonimoIdDeCookie();
        if (string.IsNullOrEmpty(anonimoId))
        {
            return NoContent();
        }

        await _fusionar.EjecutarAsync(_identidad.UsuarioIdActual!, anonimoId, ct);
        _identidad.EliminarCookieAnonimo();

        return NoContent();
    }
}
