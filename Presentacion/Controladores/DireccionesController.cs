using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;
using VoltajeModa.Presentacion.Dtos.Pedidos;
using VoltajeModa.Presentacion.Servicios;

namespace VoltajeModa.Presentacion.Controladores;

[ApiController]
[Route("api/direcciones")]
public class DireccionesController : ControllerBase
{
    private readonly CrearDireccionCasoDeUso _crear;
    private readonly ListarMisDireccionesCasoDeUso _listarMias;
    private readonly MarcarDireccionPredeterminadaCasoDeUso _marcarPredeterminada;
    private readonly ContextoIdentidadHttp _identidad;

    public DireccionesController(
        CrearDireccionCasoDeUso crear, ListarMisDireccionesCasoDeUso listarMias,
        MarcarDireccionPredeterminadaCasoDeUso marcarPredeterminada, ContextoIdentidadHttp identidad)
    {
        _crear = crear;
        _listarMias = listarMias;
        _marcarPredeterminada = marcarPredeterminada;
        _identidad = identidad;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<DireccionDto>>> ListarMias(CancellationToken ct)
    {
        var direcciones = await _listarMias.EjecutarAsync(_identidad.ObtenerOAsignarTitular(), ct);
        return Ok(direcciones.Select(DireccionDto.DesdeDominio).ToList());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<DireccionDto>> Crear([FromBody] CrearDireccionRequest request, CancellationToken ct)
    {
        var direccion = await _crear.EjecutarAsync(
            _identidad.ObtenerOAsignarTitular(), request.Etiqueta, request.NombreDestinatario, request.Telefono,
            request.Calle, request.Ciudad, request.CodigoPostal, request.EsPredeterminada, ct);

        var dto = DireccionDto.DesdeDominio(direccion);
        return CreatedAtAction(nameof(ListarMias), dto);
    }

    [HttpPatch("{id:int}/predeterminada")]
    [AllowAnonymous]
    public async Task<IActionResult> MarcarPredeterminada(int id, CancellationToken ct)
    {
        await _marcarPredeterminada.EjecutarAsync(_identidad.ObtenerOAsignarTitular(), id, ct);
        return NoContent();
    }
}
