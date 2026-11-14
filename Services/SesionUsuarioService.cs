using System.Text.Json;
using Microsoft.JSInterop;

namespace VoltajeModa.Services;

public class SesionUsuarioService
{
    private const string ClaveSesion = "voltaje_sesion";
    private readonly IJSRuntime _js;
    private readonly ILogger<SesionUsuarioService> _logger;

    public SesionUsuarioService(IJSRuntime js, ILogger<SesionUsuarioService> logger)
    {
        _js = js;
        _logger = logger;
    }

    public string? Token { get; private set; }
    public string? Correo { get; private set; }
    public string? Rol { get; private set; }
    public bool EstaAutenticado => !string.IsNullOrEmpty(Token);
    public bool Inicializada { get; private set; }

    public event Action? OnChange;

    public async Task IniciarSesionAsync(string token, string correo, string rol)
    {
        var sesion = new SesionPersistida(token, correo, rol);
        await _js.InvokeVoidAsync("localStorage.setItem", ClaveSesion, JsonSerializer.Serialize(sesion));

        Token = token;
        Correo = correo;
        Rol = rol;
        Inicializada = true;
        OnChange?.Invoke();
    }

    public async Task RestaurarAsync()
    {
        var json = await _js.InvokeAsync<string?>("localStorage.getItem", ClaveSesion);
        if (string.IsNullOrWhiteSpace(json))
        {
            Inicializada = true;
            OnChange?.Invoke();
            return;
        }

        SesionPersistida? sesion;
        try
        {
            sesion = JsonSerializer.Deserialize<SesionPersistida>(json);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "La sesión persistida no tiene un formato válido; se eliminará.");
            await _js.InvokeVoidAsync("localStorage.removeItem", ClaveSesion);
            Inicializada = true;
            OnChange?.Invoke();
            return;
        }

        if (sesion is null || string.IsNullOrWhiteSpace(sesion.Token)
            || string.IsNullOrWhiteSpace(sesion.Correo) || string.IsNullOrWhiteSpace(sesion.Rol))
        {
            _logger.LogWarning("La sesión persistida está incompleta; se eliminará.");
            await _js.InvokeVoidAsync("localStorage.removeItem", ClaveSesion);
            Inicializada = true;
            OnChange?.Invoke();
            return;
        }

        Token = sesion.Token;
        Correo = sesion.Correo;
        Rol = sesion.Rol;
        Inicializada = true;
        OnChange?.Invoke();
    }

    public async Task CerrarSesionAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", ClaveSesion);
        Token = null;
        Correo = null;
        Rol = null;
        Inicializada = true;
        OnChange?.Invoke();
    }

    private sealed record SesionPersistida(string Token, string Correo, string Rol);
}
