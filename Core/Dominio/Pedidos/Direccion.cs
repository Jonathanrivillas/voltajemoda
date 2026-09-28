using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos.Excepciones;

namespace VoltajeModa.Core.Dominio.Pedidos;

public class Direccion
{
    private Direccion(
        int id, Titular titular, string etiqueta, string? nombreDestinatario, string? telefono,
        string calle, string ciudad, string codigoPostal, bool esPredeterminada)
    {
        Id = id;
        Titular = titular;
        Etiqueta = etiqueta;
        NombreDestinatario = nombreDestinatario;
        Telefono = telefono;
        Calle = calle;
        Ciudad = ciudad;
        CodigoPostal = codigoPostal;
        EsPredeterminada = esPredeterminada;
    }

    public int Id { get; private set; }
    public Titular Titular { get; private set; }
    public string Etiqueta { get; private set; }
    public string? NombreDestinatario { get; private set; }
    public string? Telefono { get; private set; }
    public string Calle { get; private set; }
    public string Ciudad { get; private set; }
    public string CodigoPostal { get; private set; }
    public bool EsPredeterminada { get; private set; }

    public static Direccion Crear(
        Titular titular, string? etiqueta, string? nombreDestinatario, string? telefono,
        string calle, string ciudad, string codigoPostal, bool esPredeterminada = false)
    {
        if (string.IsNullOrWhiteSpace(calle) || string.IsNullOrWhiteSpace(ciudad) || string.IsNullOrWhiteSpace(codigoPostal))
        {
            throw new DireccionInvalidaException("Calle, ciudad y código postal son obligatorios.");
        }

        return new Direccion(
            0, titular, string.IsNullOrWhiteSpace(etiqueta) ? "Casa" : etiqueta.Trim(), nombreDestinatario, telefono,
            calle.Trim(), ciudad.Trim(), codigoPostal.Trim(), esPredeterminada);
    }

    internal static Direccion Reconstituir(
        int id, Titular titular, string etiqueta, string? nombreDestinatario, string? telefono,
        string calle, string ciudad, string codigoPostal, bool esPredeterminada)
        => new(id, titular, etiqueta, nombreDestinatario, telefono, calle, ciudad, codigoPostal, esPredeterminada);

    public void MarcarComoPredeterminada() => EsPredeterminada = true;

    public void QuitarPredeterminada() => EsPredeterminada = false;

    internal void ReasignarTitular(Titular nuevoTitular) => Titular = nuevoTitular;

    internal void AsignarId(int id) => Id = id;
}
