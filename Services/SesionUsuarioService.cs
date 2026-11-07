namespace VoltajeModa.Services;

public class SesionUsuarioService
{
    public string? Token { get; set; }
    public string? Correo { get; private set; }
    public string? Rol { get; private set; }
    public bool EstaAutenticado => !string.IsNullOrEmpty(Token);

    public event Action? OnChange;

    public void IniciarSesion(string token, string correo, string rol)
    {
        Token = token;
        Correo = correo;
        Rol = rol;
        OnChange?.Invoke();
    }

    public void CerrarSesion()
    {
        Token = null;
        Correo = null;
        Rol = null;
        OnChange?.Invoke();
    }
}
