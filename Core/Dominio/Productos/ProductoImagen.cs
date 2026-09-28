namespace VoltajeModa.Core.Dominio.Productos;

public class ProductoImagen
{
    internal ProductoImagen(int id, string url, int orden)
    {
        Id = id;
        Url = url;
        Orden = orden;
    }

    public int Id { get; private set; }
    public string Url { get; private set; }
    public int Orden { get; private set; }
}
