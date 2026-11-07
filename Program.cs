using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using VoltajeModa.Configuration;
using VoltajeModa.Components;
using VoltajeModa.HttpServices;
using VoltajeModa.Services;

var culturaPorDefecto = new CultureInfo("es-CO");
CultureInfo.DefaultThreadCurrentCulture = culturaPorDefecto;
CultureInfo.DefaultThreadCurrentUICulture = culturaPorDefecto;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<SesionUsuarioService>();
builder.Services.AddScoped<CarritoEstadoService>();
builder.Services.AddScoped<ProductosDestacadosService>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        if (!context.Request.Path.StartsWithSegments("/Account"))
            return RateLimitPartition.GetNoLimiter("sin-limite");

        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
});

var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>()
    ?? new ApiSettings { BaseUrl = "http://localhost:5181/api/" };
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddHttpClient<IProductoApiClient, ProductoApiClient>(client =>
    client.BaseAddress = new Uri(apiSettings.BaseUrl));
builder.Services.AddHsts(options => options.MaxAge = TimeSpan.FromDays(365));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseRateLimiter();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
