using VoltajeModa.Core.Dominio.Inventario;

namespace VoltajeModa.Core.Aplicacion.Inventario.Dtos;

public record FiltroVariantes(int? CategoriaId, string? Busqueda, EstadoStock? Estado);

public record VarianteInventarioLectura(
    int ProductoVarianteId, int ProductoId, string ProductoNombre, string ImagenUrl,
    string Talla, string Color, int Stock, int CategoriaId, string CategoriaNombre);
