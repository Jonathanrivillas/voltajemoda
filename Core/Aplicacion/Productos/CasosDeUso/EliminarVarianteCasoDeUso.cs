using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;

public class EliminarVarianteCasoDeUso
{
    private readonly IRepositorioProductos _productos;
    private readonly IVerificadorUsoVariante _verificadorUso;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public EliminarVarianteCasoDeUso(IRepositorioProductos productos, IVerificadorUsoVariante verificadorUso, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _productos = productos;
        _verificadorUso = verificadorUso;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task EjecutarAsync(int productoId, int varianteId, CancellationToken ct)
    {
        var producto = await _productos.ObtenerPorIdAsync(productoId, ct) ?? throw new ProductoNoEncontradoException(productoId);

        if (!producto.Variantes.Any(v => v.Id == varianteId))
        {
            throw new VarianteNoEncontradaException(productoId, varianteId);
        }

        if (await _verificadorUso.EstaEnUsoAsync(varianteId, ct))
        {
            throw new VarianteEnUsoException();
        }

        producto.EliminarVariante(varianteId);
        await _productos.ActualizarAsync(producto, ct);
        await _unidadDeTrabajo.GuardarCambiosAsync(ct);
    }
}
