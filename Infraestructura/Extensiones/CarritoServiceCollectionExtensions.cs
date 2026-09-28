using VoltajeModa.Core.Aplicacion.Carritos.CasosDeUso;
using VoltajeModa.Core.Aplicacion.Carritos.Interfaces;
using VoltajeModa.Infraestructura.Persistencia.Carritos.Repositorios;

namespace VoltajeModa.Infraestructura.Extensiones;

public static class CarritoServiceCollectionExtensions
{
    public static IServiceCollection AddCarritoHexagonal(this IServiceCollection services)
    {
        services.AddScoped<IRepositorioCarritos, RepositorioCarritosEf>();
        services.AddScoped<IConsultaVariantesCarrito, ConsultaVariantesCarritoEf>();

        services.AddScoped<ObtenerCarritoCasoDeUso>();
        services.AddScoped<AgregarItemCarritoCasoDeUso>();
        services.AddScoped<ActualizarCantidadItemCasoDeUso>();
        services.AddScoped<QuitarItemCarritoCasoDeUso>();
        services.AddScoped<VaciarCarritoCasoDeUso>();
        services.AddScoped<FusionarIdentidadInvitadoCasoDeUso>();

        return services;
    }
}
