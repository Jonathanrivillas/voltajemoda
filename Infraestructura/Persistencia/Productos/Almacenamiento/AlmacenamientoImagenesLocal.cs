using VoltajeModa.Core.Aplicacion.Productos.Interfaces;

namespace VoltajeModa.Infraestructura.Persistencia.Productos.Almacenamiento;

// Mismo destino que ya usaba el panel de administración: wwwroot/uploads/productos, con un nombre
// de archivo generado (GUID) para no pisar archivos existentes. Si el proyecto migra a Azure Blob
// Storage (ver backlog del README), esta es la única clase que habría que reemplazar.
public class AlmacenamientoImagenesLocal : IAlmacenamientoImagenes
{
    private readonly IWebHostEnvironment _entornoWeb;

    public AlmacenamientoImagenesLocal(IWebHostEnvironment entornoWeb)
    {
        _entornoWeb = entornoWeb;
    }

    public async Task<string> GuardarAsync(Stream contenido, string extension, CancellationToken ct)
    {
        var carpeta = Path.Combine(_entornoWeb.WebRootPath, "uploads", "productos");
        Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

        await using (var destino = File.Create(rutaCompleta))
        {
            await contenido.CopyToAsync(destino, ct);
        }

        return $"/uploads/productos/{nombreArchivo}";
    }
}
