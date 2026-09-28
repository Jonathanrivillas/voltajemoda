using VoltajeModa.Core.Aplicacion.Productos.CasosDeUso;
using VoltajeModa.Core.Aplicacion.Productos.Interfaces;
using VoltajeModa.Infraestructura.Persistencia.Productos.Almacenamiento;
using VoltajeModa.Infraestructura.Persistencia.Productos.Repositorios;

namespace VoltajeModa.Infraestructura.Extensiones;

public static class ProductosServiceCollectionExtensions
{
    public static IServiceCollection AddProductosHexagonal(this IServiceCollection services)
    {
        services.AddScoped<IRepositorioProductos, RepositorioProductosEf>();
        services.AddScoped<IRepositorioCategorias, RepositorioCategoriasEf>();
        services.AddScoped<IVerificadorUsoVariante, VerificadorUsoVarianteEf>();
        services.AddScoped<IAlmacenamientoImagenes, AlmacenamientoImagenesLocal>();

        services.AddScoped<CrearProductoCasoDeUso>();
        services.AddScoped<ActualizarProductoCasoDeUso>();
        services.AddScoped<ObtenerProductoPorIdCasoDeUso>();
        services.AddScoped<ListarProductosCasoDeUso>();
        services.AddScoped<EliminarProductoCasoDeUso>();
        services.AddScoped<PonerProductoEnOfertaCasoDeUso>();
        services.AddScoped<QuitarOfertaProductoCasoDeUso>();
        services.AddScoped<AgregarVarianteCasoDeUso>();
        services.AddScoped<AgregarImagenCasoDeUso>();
        services.AddScoped<EliminarVarianteCasoDeUso>();
        services.AddScoped<EliminarImagenCasoDeUso>();
        services.AddScoped<SubirImagenPrincipalCasoDeUso>();
        services.AddScoped<SubirImagenAdicionalCasoDeUso>();
        services.AddScoped<ListarCategoriasCasoDeUso>();
        services.AddScoped<CrearCategoriaCasoDeUso>();
        services.AddScoped<ActualizarCategoriaCasoDeUso>();
        services.AddScoped<EliminarCategoriaCasoDeUso>();

        return services;
    }
}
