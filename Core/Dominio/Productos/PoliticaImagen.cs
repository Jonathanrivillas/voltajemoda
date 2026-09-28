using VoltajeModa.Core.Dominio.Productos.Excepciones;

namespace VoltajeModa.Core.Dominio.Productos;

// Mismas reglas que ya validaba Components/Pages/Admin/ProductoForm.razor al subir un archivo.
public static class PoliticaImagen
{
    public const long TamanoMaximoBytes = 5 * 1024 * 1024;
    public static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    public static void ValidarArchivo(string extension, long tamanoBytes)
    {
        if (!ExtensionesPermitidas.Contains(extension.ToLowerInvariant()))
        {
            throw new ProductoInvalidoException("Formato no soportado. Usa JPG, PNG, WEBP o GIF.");
        }

        if (tamanoBytes > TamanoMaximoBytes)
        {
            throw new ProductoInvalidoException("La imagen no puede superar los 5 MB.");
        }
    }
}
