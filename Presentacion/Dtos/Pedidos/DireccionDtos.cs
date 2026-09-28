using VoltajeModa.Core.Dominio.Pedidos;

namespace VoltajeModa.Presentacion.Dtos.Pedidos;

public record DireccionDto(
    int Id, string Etiqueta, string? NombreDestinatario, string? Telefono,
    string Calle, string Ciudad, string CodigoPostal, bool EsPredeterminada)
{
    public static DireccionDto DesdeDominio(Direccion d) => new(
        d.Id, d.Etiqueta, d.NombreDestinatario, d.Telefono, d.Calle, d.Ciudad, d.CodigoPostal, d.EsPredeterminada);
}

public record CrearDireccionRequest(
    string? Etiqueta, string? NombreDestinatario, string? Telefono,
    string Calle, string Ciudad, string CodigoPostal, bool EsPredeterminada);
