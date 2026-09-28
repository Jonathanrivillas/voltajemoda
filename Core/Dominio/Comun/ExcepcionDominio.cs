namespace VoltajeModa.Core.Dominio.Comun;

public abstract class ExcepcionDominio : Exception
{
    protected ExcepcionDominio(string mensaje, int codigoHttp) : base(mensaje)
    {
        CodigoHttp = codigoHttp;
    }

    public int CodigoHttp { get; }
}
