using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Pedidos.Mapeadores;

public static class DireccionMapeador
{
    public static Direccion ADominio(EfModels.Direccion entidad)
    {
        var titular = entidad.UsuarioId is not null
            ? Titular.DeUsuario(entidad.UsuarioId)
            : Titular.DeInvitado(entidad.AnonimoId!);

        return Direccion.Reconstituir(
            entidad.Id, titular, entidad.Etiqueta, entidad.NombreDestinatario, entidad.Telefono,
            entidad.Calle, entidad.Ciudad, entidad.CodigoPostal, entidad.EsPredeterminada);
    }

    public static EfModels.Direccion AEntidadNueva(Direccion direccion) => new()
    {
        UsuarioId = direccion.Titular.UsuarioId,
        AnonimoId = direccion.Titular.AnonimoId,
        Etiqueta = direccion.Etiqueta,
        NombreDestinatario = direccion.NombreDestinatario,
        Telefono = direccion.Telefono,
        Calle = direccion.Calle,
        Ciudad = direccion.Ciudad,
        CodigoPostal = direccion.CodigoPostal,
        EsPredeterminada = direccion.EsPredeterminada
    };

    public static void VolcarEnEntidad(Direccion direccion, EfModels.Direccion entidad)
    {
        entidad.UsuarioId = direccion.Titular.UsuarioId;
        entidad.AnonimoId = direccion.Titular.AnonimoId;
        entidad.Etiqueta = direccion.Etiqueta;
        entidad.NombreDestinatario = direccion.NombreDestinatario;
        entidad.Telefono = direccion.Telefono;
        entidad.Calle = direccion.Calle;
        entidad.Ciudad = direccion.Ciudad;
        entidad.CodigoPostal = direccion.CodigoPostal;
        entidad.EsPredeterminada = direccion.EsPredeterminada;
    }
}
