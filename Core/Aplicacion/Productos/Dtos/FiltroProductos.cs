namespace VoltajeModa.Core.Aplicacion.Productos.Dtos;

public record FiltroProductos(
    int? CategoriaId,
    string? Busqueda,
    bool? Destacado,
    bool? EnOferta,
    bool? EsNuevo,
    int Pagina = 1,
    int TamanoPagina = 20);
