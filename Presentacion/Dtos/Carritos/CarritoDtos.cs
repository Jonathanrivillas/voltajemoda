using VoltajeModa.Core.Dominio.Carritos;

namespace VoltajeModa.Presentacion.Dtos.Carritos;

public record ItemCarritoDto(int ProductoVarianteId, int Cantidad)
{
    public static ItemCarritoDto DesdeDominio(ItemCarrito i) => new(i.ProductoVarianteId, i.Cantidad);
}

public record CarritoDto(int Id, IReadOnlyList<ItemCarritoDto> Items)
{
    public static CarritoDto DesdeDominio(Carrito c) => new(c.Id, c.Items.Select(ItemCarritoDto.DesdeDominio).ToList());
}

public record AgregarItemRequest(int ProductoVarianteId, int Cantidad);

public record ActualizarCantidadRequest(int Cantidad);
