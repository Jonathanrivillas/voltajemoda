using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;
using VoltajeModa.Core.Aplicacion.Productos.Dtos;
using VoltajeModa.Presentacion.Dtos.Comun;
using VoltajeModa.Presentacion.Dtos.Productos;

namespace VoltajeModa.Presentacion.Controladores;

[ApiController]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    private readonly CrearProductoCasoDeUso _crear;
    private readonly ActualizarProductoCasoDeUso _actualizar;
    private readonly ObtenerProductoPorIdCasoDeUso _obtenerPorId;
    private readonly ListarProductosCasoDeUso _listar;
    private readonly EliminarProductoCasoDeUso _eliminar;
    private readonly PonerProductoEnOfertaCasoDeUso _ponerEnOferta;
    private readonly QuitarOfertaProductoCasoDeUso _quitarOferta;
    private readonly AgregarVarianteCasoDeUso _agregarVariante;
    private readonly AgregarImagenCasoDeUso _agregarImagen;

    public ProductosController(
        CrearProductoCasoDeUso crear, ActualizarProductoCasoDeUso actualizar, ObtenerProductoPorIdCasoDeUso obtenerPorId,
        ListarProductosCasoDeUso listar, EliminarProductoCasoDeUso eliminar, PonerProductoEnOfertaCasoDeUso ponerEnOferta,
        QuitarOfertaProductoCasoDeUso quitarOferta, AgregarVarianteCasoDeUso agregarVariante, AgregarImagenCasoDeUso agregarImagen)
    {
        _crear = crear;
        _actualizar = actualizar;
        _obtenerPorId = obtenerPorId;
        _listar = listar;
        _eliminar = eliminar;
        _ponerEnOferta = ponerEnOferta;
        _quitarOferta = quitarOferta;
        _agregarVariante = agregarVariante;
        _agregarImagen = agregarImagen;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ListaPaginadaDto<ProductoResumenDto>>> Listar(
        [FromQuery] int? categoriaId, [FromQuery] string? busqueda, [FromQuery] bool? destacado,
        [FromQuery] bool? enOferta, [FromQuery] bool? esNuevo, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var filtro = new FiltroProductos(categoriaId, busqueda, destacado, enOferta, esNuevo, pagina, tamanoPagina);
        var (items, total) = await _listar.EjecutarAsync(filtro, ct);

        return Ok(new ListaPaginadaDto<ProductoResumenDto>(
            items.Select(ProductoResumenDto.DesdeDominio).ToList(), total, pagina, tamanoPagina));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductoDetalleDto>> ObtenerPorId(int id, CancellationToken ct)
    {
        var producto = await _obtenerPorId.EjecutarAsync(id, ct);
        return Ok(ProductoDetalleDto.DesdeDominio(producto));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProductoDetalleDto>> Crear([FromBody] CrearProductoRequest request, CancellationToken ct)
    {
        var producto = await _crear.EjecutarAsync(
            request.Nombre, request.Descripcion, request.Precio, request.ImagenUrl,
            request.CategoriaId, request.Destacado, request.EsNuevo, ct);

        var dto = ProductoDetalleDto.DesdeDominio(producto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProductoRequest request, CancellationToken ct)
    {
        await _actualizar.EjecutarAsync(
            id, request.Nombre, request.Descripcion, request.Precio, request.ImagenUrl,
            request.CategoriaId, request.Destacado, request.EsNuevo, ct);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await _eliminar.EjecutarAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/oferta")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProductoDetalleDto>> PonerEnOferta(int id, [FromBody] PonerOfertaRequest request, CancellationToken ct)
    {
        var producto = await _ponerEnOferta.EjecutarAsync(id, request.PrecioOferta, ct);
        return Ok(ProductoDetalleDto.DesdeDominio(producto));
    }

    [HttpDelete("{id:int}/oferta")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> QuitarOferta(int id, CancellationToken ct)
    {
        await _quitarOferta.EjecutarAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/variantes")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<VarianteDto>> AgregarVariante(int id, [FromBody] CrearVarianteRequest request, CancellationToken ct)
    {
        var variante = await _agregarVariante.EjecutarAsync(id, request.Talla, request.Color, request.StockInicial, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, VarianteDto.DesdeDominio(variante));
    }

    [HttpPost("{id:int}/imagenes")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ImagenDto>> AgregarImagen(int id, [FromBody] CrearImagenRequest request, CancellationToken ct)
    {
        var imagen = await _agregarImagen.EjecutarAsync(id, request.Url, request.Orden, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, ImagenDto.DesdeDominio(imagen));
    }
}
