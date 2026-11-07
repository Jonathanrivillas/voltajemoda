namespace VoltajeModa.Services;

public class ProductosDestacadosService
{
    private readonly HashSet<int> _productoIds = new();

    public event Action? OnChange;

    public bool EstaDestacado(int productoId) => _productoIds.Contains(productoId);

    public IReadOnlyCollection<int> ObtenerIds() => _productoIds;

    public void Alternar(int productoId)
    {
        if (!_productoIds.Add(productoId))
            _productoIds.Remove(productoId);

        OnChange?.Invoke();
    }
}
