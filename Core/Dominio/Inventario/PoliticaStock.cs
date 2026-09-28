namespace VoltajeModa.Core.Dominio.Inventario;

public enum EstadoStock
{
    SinStock,
    StockBajo,
    EnStock
}

public static class PoliticaStock
{
    public const int UmbralStockBajo = 5;

    public static EstadoStock EstadoDe(int stock) => stock switch
    {
        <= 0 => EstadoStock.SinStock,
        _ when stock <= UmbralStockBajo => EstadoStock.StockBajo,
        _ => EstadoStock.EnStock
    };
}
