namespace VoltajeModa.Core.Aplicacion.Productos.Interfaces;

// Puerto hacia el almacenamiento de archivos. Hoy la implementación guarda en disco local
// (wwwroot/uploads/productos), igual que ya hacía el panel de administración; el día que el
// proyecto migre a Azure Blob Storage (ver backlog del README), solo cambia el adaptador.
public interface IAlmacenamientoImagenes
{
    Task<string> GuardarAsync(Stream contenido, string extension, CancellationToken ct);
}
