using VoltajeModa.Core.Aplicacion.Inventario.Dtos;
using VoltajeModa.Core.Aplicacion.Inventario.Interfaces;

namespace VoltajeModa.Core.Aplicacion.Inventario.CasosDeUso;

public class ListarVariantesInventarioCasoDeUso
{
    private readonly IConsultaVariantesInventario _variantes;

    public ListarVariantesInventarioCasoDeUso(IConsultaVariantesInventario variantes)
    {
        _variantes = variantes;
    }

    public Task<IReadOnlyList<VarianteInventarioLectura>> EjecutarAsync(FiltroVariantes filtro, CancellationToken ct)
    {
        return _variantes.ListarAsync(filtro, ct);
    }
}
