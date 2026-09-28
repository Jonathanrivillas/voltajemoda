using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Infraestructura.Persistencia.Comun;
using VoltajeModa.Presentacion.Filtros;
using VoltajeModa.Presentacion.Servicios;

namespace VoltajeModa.Infraestructura.Extensiones;

public static class ArquitecturaHexagonalServiceCollectionExtensions
{
    public static IServiceCollection AddArquitecturaHexagonal(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajoEf>();
        services.AddScoped<ContextoIdentidadHttp>();

        services.AddProductosHexagonal();
        services.AddInventarioHexagonal();
        services.AddCarritoHexagonal();
        services.AddPedidosHexagonal();

        services.AddControllers(o => o.Filters.Add<ManejadorExcepcionesFiltro>());

        return services;
    }
}
