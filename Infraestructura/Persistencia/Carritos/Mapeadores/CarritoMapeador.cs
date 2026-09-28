using VoltajeModa.Core.Dominio.Carritos;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Carritos.Mapeadores;

public static class CarritoMapeador
{
    public static Carrito ADominio(EfModels.Carrito entidad)
    {
        var titular = entidad.UsuarioId is not null
            ? Titular.DeUsuario(entidad.UsuarioId)
            : Titular.DeInvitado(entidad.AnonimoId!);

        var items = entidad.Items.Select(i => new ItemCarrito(i.Id, i.ProductoVarianteId, i.Cantidad));

        return Carrito.Reconstituir(entidad.Id, titular, items);
    }

    public static EfModels.Carrito AEntidadNueva(Carrito carrito) => new()
    {
        UsuarioId = carrito.Titular.UsuarioId,
        AnonimoId = carrito.Titular.AnonimoId,
        Items = carrito.Items
            .Select(i => new EfModels.CarritoItem { ProductoVarianteId = i.ProductoVarianteId, Cantidad = i.Cantidad })
            .ToList()
    };

    // Sincroniza la colección de items en vez de reemplazarla: actualiza cantidad de los que
    // siguen presentes, elimina (de la colección trackeada) los que ya no están en el dominio,
    // y agrega como nuevas filas los que el dominio tiene con Id == 0.
    public static void VolcarEnEntidad(Carrito carrito, EfModels.Carrito entidad)
    {
        var itemsDominioPorVariante = carrito.Items.ToDictionary(i => i.ProductoVarianteId);

        foreach (var itemEf in entidad.Items.ToList())
        {
            if (itemsDominioPorVariante.TryGetValue(itemEf.ProductoVarianteId, out var itemDominio))
            {
                itemEf.Cantidad = itemDominio.Cantidad;
            }
            else
            {
                entidad.Items.Remove(itemEf);
            }
        }

        var varianteIdsEf = entidad.Items.Select(i => i.ProductoVarianteId).ToHashSet();
        foreach (var itemDominio in carrito.Items.Where(i => !varianteIdsEf.Contains(i.ProductoVarianteId)))
        {
            entidad.Items.Add(new EfModels.CarritoItem { ProductoVarianteId = itemDominio.ProductoVarianteId, Cantidad = itemDominio.Cantidad });
        }
    }
}
