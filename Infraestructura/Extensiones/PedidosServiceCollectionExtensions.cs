using VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Infraestructura.Persistencia.Pedidos.Adaptadores;
using VoltajeModa.Infraestructura.Persistencia.Pedidos.Repositorios;

namespace VoltajeModa.Infraestructura.Extensiones;

public static class PedidosServiceCollectionExtensions
{
    // AjustadorStockInventarioAdapter depende de puertos de Inventario (IConsultaVariantesInventario,
    // IRepositorioMovimientosStock): deben estar registrados en el mismo IServiceCollection antes de
    // construir el ServiceProvider, pero el orden entre AddInventarioHexagonal()/AddPedidosHexagonal()
    // no importa (la resolución de DI es perezosa, no depende del orden de los Add*).
    public static IServiceCollection AddPedidosHexagonal(this IServiceCollection services)
    {
        services.AddScoped<IRepositorioPedidos, RepositorioPedidosEf>();
        services.AddScoped<IRepositorioDirecciones, RepositorioDireccionesEf>();
        services.AddScoped<ILectorCarritoParaCheckout, LectorCarritoParaCheckoutEf>();
        services.AddScoped<IConsultaVarianteCheckout, ConsultaVarianteCheckoutEf>();
        services.AddScoped<IAjustadorStock, AjustadorStockInventarioAdapter>();

        services.AddScoped<CrearPedidoDesdeCarritoCasoDeUso>();
        services.AddScoped<ObtenerPedidoPorIdCasoDeUso>();
        services.AddScoped<ListarMisPedidosCasoDeUso>();
        services.AddScoped<ListarPedidosCasoDeUso>();
        services.AddScoped<CambiarEstadoPedidoCasoDeUso>();
        services.AddScoped<CrearDireccionCasoDeUso>();
        services.AddScoped<ListarMisDireccionesCasoDeUso>();
        services.AddScoped<MarcarDireccionPredeterminadaCasoDeUso>();
        services.AddScoped<EliminarDireccionCasoDeUso>();
        services.AddScoped<EliminarPedidoCasoDeUso>();

        return services;
    }
}
