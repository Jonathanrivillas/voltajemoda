using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;
using VoltajeModa.Core.Aplicacion.Pedidos.Dtos;
using VoltajeModa.Core.Dominio.Pedidos;
using VoltajeModa.Presentacion.Dtos.Comun;
using VoltajeModa.Presentacion.Dtos.Pedidos;
using VoltajeModa.Presentacion.Servicios;

namespace VoltajeModa.Presentacion.Controladores;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly CrearPedidoDesdeCarritoCasoDeUso _crear;
    private readonly ObtenerPedidoPorIdCasoDeUso _obtenerPorId;
    private readonly ListarMisPedidosCasoDeUso _listarMios;
    private readonly ListarPedidosCasoDeUso _listarTodos;
    private readonly CambiarEstadoPedidoCasoDeUso _cambiarEstado;
    private readonly EliminarPedidoCasoDeUso _eliminar;
    private readonly ContextoIdentidadHttp _identidad;

    public PedidosController(
        CrearPedidoDesdeCarritoCasoDeUso crear, ObtenerPedidoPorIdCasoDeUso obtenerPorId, ListarMisPedidosCasoDeUso listarMios,
        ListarPedidosCasoDeUso listarTodos, CambiarEstadoPedidoCasoDeUso cambiarEstado, EliminarPedidoCasoDeUso eliminar,
        ContextoIdentidadHttp identidad)
    {
        _crear = crear;
        _obtenerPorId = obtenerPorId;
        _listarMios = listarMios;
        _listarTodos = listarTodos;
        _cambiarEstado = cambiarEstado;
        _eliminar = eliminar;
        _identidad = identidad;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PedidoDetalleDto>> Crear([FromBody] CrearPedidoRequest request, CancellationToken ct)
    {
        var pedido = await _crear.EjecutarAsync(_identidad.ObtenerOAsignarTitular(), request.DireccionId, ct);
        var dto = PedidoDetalleDto.DesdeDominio(pedido);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = dto.Id }, dto);
    }

    [HttpGet("mios")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PedidoResumenDto>>> ListarMios(CancellationToken ct)
    {
        var pedidos = await _listarMios.EjecutarAsync(_identidad.ObtenerOAsignarTitular(), ct);
        return Ok(pedidos.Select(PedidoResumenDto.DesdeDominio).ToList());
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PedidoDetalleDto>> ObtenerPorId(int id, CancellationToken ct)
    {
        var esAdministrador = User.IsInRole("Administrador");
        var pedido = await _obtenerPorId.EjecutarAsync(id, _identidad.ObtenerOAsignarTitular(), esAdministrador, ct);
        return Ok(PedidoDetalleDto.DesdeDominio(pedido));
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ListaPaginadaDto<PedidoResumenDto>>> Listar(
        [FromQuery] EstadoPedido? estado, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta,
        [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20, CancellationToken ct = default)
    {
        var filtro = new FiltroPedidos(estado, desde, hasta, pagina, tamanoPagina);
        var (items, total) = await _listarTodos.EjecutarAsync(filtro, ct);

        return Ok(new ListaPaginadaDto<PedidoResumenDto>(items.Select(PedidoResumenDto.DesdeDominio).ToList(), total, pagina, tamanoPagina));
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<PedidoDetalleDto>> CambiarEstado(int id, [FromBody] CambiarEstadoRequest request, CancellationToken ct)
    {
        var pedido = await _cambiarEstado.EjecutarAsync(id, request.NuevoEstado, ct);
        return Ok(PedidoDetalleDto.DesdeDominio(pedido));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await _eliminar.EjecutarAsync(id, ct);
        return NoContent();
    }
}
