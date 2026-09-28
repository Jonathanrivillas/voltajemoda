using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltajeModa.Core.Aplicacion.Inventario.CasosDeUso;
using VoltajeModa.Core.Aplicacion.Inventario.Dtos;
using VoltajeModa.Core.Dominio.Inventario;
using VoltajeModa.Presentacion.Dtos.Comun;
using VoltajeModa.Presentacion.Dtos.Inventario;

namespace VoltajeModa.Presentacion.Controladores;

[ApiController]
[Route("api/inventario")]
[Authorize(Roles = "Administrador")]
public class InventarioController : ControllerBase
{
    private readonly ListarVariantesInventarioCasoDeUso _listarVariantes;
    private readonly RegistrarMovimientoStockCasoDeUso _registrarMovimiento;
    private readonly ListarHistorialMovimientosCasoDeUso _listarHistorial;

    public InventarioController(
        ListarVariantesInventarioCasoDeUso listarVariantes,
        RegistrarMovimientoStockCasoDeUso registrarMovimiento,
        ListarHistorialMovimientosCasoDeUso listarHistorial)
    {
        _listarVariantes = listarVariantes;
        _registrarMovimiento = registrarMovimiento;
        _listarHistorial = listarHistorial;
    }

    [HttpGet("variantes")]
    public async Task<ActionResult<IReadOnlyList<VarianteInventarioDto>>> ListarVariantes(
        [FromQuery] int? categoriaId, [FromQuery] string? busqueda, [FromQuery] EstadoStock? estado, CancellationToken ct)
    {
        var variantes = await _listarVariantes.EjecutarAsync(new FiltroVariantes(categoriaId, busqueda, estado), ct);
        return Ok(variantes.Select(VarianteInventarioDto.DesdeLectura).ToList());
    }

    [HttpPost("movimientos")]
    public async Task<ActionResult<MovimientoStockDto>> RegistrarMovimiento([FromBody] RegistrarMovimientoRequest request, CancellationToken ct)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var movimiento = await _registrarMovimiento.EjecutarAsync(
            request.ProductoVarianteId, request.Tipo, request.Cantidad, request.Motivo, usuarioId, ct);

        return StatusCode(StatusCodes.Status201Created, MovimientoStockDto.DesdeDominio(movimiento));
    }

    [HttpGet("movimientos")]
    public async Task<ActionResult<ListaPaginadaDto<MovimientoStockDto>>> ListarHistorial(
        [FromQuery] int? productoVarianteId, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta,
        [FromQuery] TipoMovimiento? tipo, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20, CancellationToken ct = default)
    {
        var filtro = new FiltroHistorial(productoVarianteId, desde, hasta, tipo, pagina, tamanoPagina);
        var (items, total) = await _listarHistorial.EjecutarAsync(filtro, ct);

        return Ok(new ListaPaginadaDto<MovimientoStockDto>(
            items.Select(MovimientoStockDto.DesdeDominio).ToList(), total, pagina, tamanoPagina));
    }
}
