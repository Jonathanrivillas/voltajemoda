using System.Text.Json.Serialization;
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

        services.AddControllers(o => o.Filters.Add<ManejadorExcepcionesFiltro>())
            // Permite mandar los enums en el JSON como texto legible (ej. "Enviado", "Salida")
            // en vez de solo el número subyacente (1, 0...), tanto al recibir como al responder.
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return services;
    }
}
