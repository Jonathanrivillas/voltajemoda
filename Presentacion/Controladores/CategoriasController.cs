using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;
using VoltajeModa.Presentacion.Dtos.Productos;

namespace VoltajeModa.Presentacion.Controladores;

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly ListarCategoriasCasoDeUso _listar;
    private readonly CrearCategoriaCasoDeUso _crear;
    private readonly ActualizarCategoriaCasoDeUso _actualizar;
    private readonly EliminarCategoriaCasoDeUso _eliminar;

    public CategoriasController(
        ListarCategoriasCasoDeUso listar, CrearCategoriaCasoDeUso crear,
        ActualizarCategoriaCasoDeUso actualizar, EliminarCategoriaCasoDeUso eliminar)
    {
        _listar = listar;
        _crear = crear;
        _actualizar = actualizar;
        _eliminar = eliminar;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CategoriaDto>>> Listar(CancellationToken ct)
    {
        var categorias = await _listar.EjecutarAsync(ct);
        return Ok(categorias.Select(CategoriaDto.DesdeDominio).ToList());
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<CategoriaDto>> Crear([FromBody] CrearCategoriaRequest request, CancellationToken ct)
    {
        var categoria = await _crear.EjecutarAsync(request.Nombre, ct);
        var dto = CategoriaDto.DesdeDominio(categoria);
        return CreatedAtAction(nameof(Listar), dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<CategoriaDto>> Actualizar(int id, [FromBody] ActualizarCategoriaRequest request, CancellationToken ct)
    {
        var categoria = await _actualizar.EjecutarAsync(id, request.Nombre, ct);
        return Ok(CategoriaDto.DesdeDominio(categoria));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await _eliminar.EjecutarAsync(id, ct);
        return NoContent();
    }
}
