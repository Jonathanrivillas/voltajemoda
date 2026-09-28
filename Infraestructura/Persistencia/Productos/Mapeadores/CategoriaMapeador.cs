using VoltajeModa.Core.Dominio.Productos;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Productos.Mapeadores;

public static class CategoriaMapeador
{
    public static Categoria ADominio(EfModels.Categoria entidad) => Categoria.Reconstituir(entidad.Id, entidad.Nombre);

    public static EfModels.Categoria AEntidadNueva(Categoria categoria) => new() { Nombre = categoria.Nombre };
}
