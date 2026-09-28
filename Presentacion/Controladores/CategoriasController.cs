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

    public CategoriasController(ListarCategoriasCasoDeUso listar, CrearCategoriaCasoDeUso crear)
    {
        _listar = listar;
        _crear = crear;
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
}
