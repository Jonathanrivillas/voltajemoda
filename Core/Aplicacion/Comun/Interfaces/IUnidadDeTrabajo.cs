namespace VoltajeModa.Core.Aplicacion.Comun.Interfaces;

public interface IUnidadDeTrabajo
{
    Task GuardarCambiosAsync(CancellationToken ct = default);
}
