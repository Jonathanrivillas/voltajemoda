using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Core.Dominio.Pedidos.Excepciones;

public class DireccionEnUsoException : ExcepcionDominio
{
    public DireccionEnUsoException()
        : base("No puedes eliminar esta dirección porque tiene pedidos asociados.", 409)
    {
    }
}
