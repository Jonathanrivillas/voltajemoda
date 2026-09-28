using VoltajeModa.Core.Dominio.Carritos.Excepciones;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;

namespace VoltajeModa.Core.Dominio.Carritos;

public class Carrito
{
    private readonly List<ItemCarrito> _items = new();

    private Carrito(int id, Titular titular)
    {
        Id = id;
        Titular = titular;
    }

    public int Id { get; private set; }
    public Titular Titular { get; private set; }
    public IReadOnlyList<ItemCarrito> Items => _items.AsReadOnly();

    public static Carrito Crear(Titular titular) => new(0, titular);

    internal static Carrito Reconstituir(int id, Titular titular, IEnumerable<ItemCarrito> items)
    {
        var carrito = new Carrito(id, titular);
        carrito._items.AddRange(items);
        return carrito;
    }

    public void AgregarItem(int productoVarianteId, int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new CantidadInvalidaException("La cantidad debe ser mayor a 0.");
        }

        var existente = _items.FirstOrDefault(i => i.ProductoVarianteId == productoVarianteId);
        if (existente is not null)
        {
            existente.SumarCantidad(cantidad);
        }
        else
        {
            _items.Add(new ItemCarrito(0, productoVarianteId, cantidad));
        }
    }

    public void ActualizarCantidad(int productoVarianteId, int cantidad)
    {
        var existente = _items.FirstOrDefault(i => i.ProductoVarianteId == productoVarianteId)
            ?? throw new ItemCarritoNoEncontradoException(productoVarianteId);

        if (cantidad <= 0)
        {
            _items.Remove(existente);
        }
        else
        {
            existente.FijarCantidad(cantidad);
        }
    }

    public void QuitarItem(int productoVarianteId)
    {
        var existente = _items.FirstOrDefault(i => i.ProductoVarianteId == productoVarianteId)
            ?? throw new ItemCarritoNoEncontradoException(productoVarianteId);

        _items.Remove(existente);
    }

    public void Vaciar() => _items.Clear();

    // Suma cantidades por variante coincidente; el carrito origen (típicamente el anónimo) no se
    // modifica aquí, el caso de uso que orquesta la fusión es quien lo elimina tras persistir esto.
    public void FusionarCon(Carrito origen)
    {
        foreach (var item in origen.Items)
        {
            AgregarItem(item.ProductoVarianteId, item.Cantidad);
        }
    }

    public bool EstaVacio() => _items.Count == 0;

    internal void AsignarId(int id) => Id = id;
}
