using System.Globalization;
using System.Threading.RateLimiting;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using VoltajeModa.Components;
using VoltajeModa.Components.Account;
using VoltajeModa.Data;
using VoltajeModa.Services;

var culturaPorDefecto = new CultureInfo("es-CO");
CultureInfo.DefaultThreadCurrentCulture = culturaPorDefecto;
CultureInfo.DefaultThreadCurrentUICulture = culturaPorDefecto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
});
authenticationBuilder.AddIdentityCookies();

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    authenticationBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.Events.OnRedirectToAuthorizationEndpoint = context =>
        {
            var redirectUri = QueryHelpers.AddQueryString(context.RedirectUri, "prompt", "select_account");
            context.Response.Redirect(redirectUri);
            return Task.CompletedTask;
        };
    });
}

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ApplicationDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;

        options.Password.RequiredLength = 10;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        if (!httpContext.Request.Path.StartsWithSegments("/Account"))
        {
            return RateLimitPartition.GetNoLimiter("sin-limite");
        }

        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
}
else
{
    var acsConnectionString = builder.Configuration["AzureCommunicationServices:ConnectionString"]
        ?? throw new InvalidOperationException("Falta configurar AzureCommunicationServices:ConnectionString.");
    builder.Services.AddSingleton(new EmailClient(acsConnectionString));
    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, AzureEmailSender>();
}

builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<CarritoEstadoService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<InventarioService>();
builder.Services.AddScoped<EstadisticasService>();

builder.Services.AddHsts(options => options.MaxAge = TimeSpan.FromDays(365));

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeeder.SeedRolesAsync(roleManager);

    // Datos de catálogo de prueba y el usuario administrador de contraseña conocida
    // son solo para desarrollo local: en producción se cargan/crean manualmente.
    if (app.Environment.IsDevelopment())
    {
        DbSeeder.Seed(db);

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        await IdentitySeeder.SeedDevAdminAsync(userManager, logger);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// MapStaticAssets() (abajo) solo sirve assets conocidos en tiempo de compilación (el manifiesto
// de fingerprinting). Los archivos que se suben en tiempo de ejecución (imágenes de productos)
// necesitan el middleware clásico, que lee directo del disco en cada request.
app.UseStaticFiles();

app.UseAntiforgery();

app.UseRateLimiter();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
