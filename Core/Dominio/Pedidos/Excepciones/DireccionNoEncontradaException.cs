using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Pedidos.Excepciones;

public class DireccionNoEncontradaException : ExcepcionDominio
{
    public DireccionNoEncontradaException(int id) : base($"No se encontró la dirección con id {id}.", 404)
    {
    }
}
