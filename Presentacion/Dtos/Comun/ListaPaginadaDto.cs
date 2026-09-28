namespace VoltajeModa.Presentacion.Dtos.Comun;

public record ListaPaginadaDto<T>(IReadOnlyList<T> Items, int Total, int Pagina, int TamanoPagina);
