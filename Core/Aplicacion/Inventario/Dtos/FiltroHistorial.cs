using VoltajeModa.Core.Dominio.Inventario;

namespace VoltajeModa.Core.Aplicacion.Inventario.Dtos;

public record FiltroHistorial(
    int? ProductoVarianteId, DateTime? Desde, DateTime? Hasta, TipoMovimiento? Tipo,
    int Pagina = 1, int TamanoPagina = 20);
