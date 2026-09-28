using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Dominio.Productos;

public class Categoria
{
    private Categoria(int id, string nombre)
    {
        Id = id;
        Nombre = nombre;
    }

    public int Id { get; private set; }
    public string Nombre { get; private set; }

    public static Categoria Crear(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ProductoInvalidoException("El nombre de la categoría no puede estar vacío.");
        }

        return new Categoria(0, nombre.Trim());
    }

    internal static Categoria Reconstituir(int id, string nombre) => new(id, nombre);

    internal void AsignarId(int id) => Id = id;
}
