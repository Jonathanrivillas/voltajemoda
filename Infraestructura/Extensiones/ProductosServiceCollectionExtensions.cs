using VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Infraestructura.Persistencia.Productos.Repositorios;

namespace VoltajeModa.Infraestructura.Extensiones;

public static class ProductosServiceCollectionExtensions
{
    public static IServiceCollection AddProductosHexagonal(this IServiceCollection services)
    {
        services.AddScoped<IRepositorioProductos, RepositorioProductosEf>();
        services.AddScoped<IRepositorioCategorias, RepositorioCategoriasEf>();

        services.AddScoped<CrearProductoCasoDeUso>();
        services.AddScoped<ActualizarProductoCasoDeUso>();
        services.AddScoped<ObtenerProductoPorIdCasoDeUso>();
        services.AddScoped<ListarProductosCasoDeUso>();
        services.AddScoped<EliminarProductoCasoDeUso>();
        services.AddScoped<PonerProductoEnOfertaCasoDeUso>();
        services.AddScoped<QuitarOfertaProductoCasoDeUso>();
        services.AddScoped<AgregarVarianteCasoDeUso>();
        services.AddScoped<AgregarImagenCasoDeUso>();
        services.AddScoped<ListarCategoriasCasoDeUso>();
        services.AddScoped<CrearCategoriaCasoDeUso>();

        return services;
    }
}
