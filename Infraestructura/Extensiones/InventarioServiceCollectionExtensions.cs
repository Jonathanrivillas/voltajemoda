using VoltajeModa.Core.Aplicacion.Inventario.CasosDeUso;
using VoltajeModa.Core.Aplicacion.Inventario.Interfaces;
using VoltajeModa.Infraestructura.Persistencia.Inventario.Repositorios;

namespace VoltajeModa.Infraestructura.Extensiones;

public static class InventarioServiceCollectionExtensions
{
    public static IServiceCollection AddInventarioHexagonal(this IServiceCollection services)
    {
        services.AddScoped<IRepositorioMovimientosStock, RepositorioMovimientosStockEf>();
        services.AddScoped<IConsultaVariantesInventario, ConsultaVariantesInventarioEf>();

        services.AddScoped<RegistrarMovimientoStockCasoDeUso>();
        services.AddScoped<ListarVariantesInventarioCasoDeUso>();
        services.AddScoped<ListarHistorialMovimientosCasoDeUso>();

        return services;
    }
}
