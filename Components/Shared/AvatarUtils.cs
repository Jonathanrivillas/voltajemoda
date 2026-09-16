namespace VoltajeModa.Components.Shared;

public static class AvatarUtils
{
    public static string CalcularIniciales(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return "?";
        }

        var partes = texto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 1)
        {
            return partes[0][..Math.Min(2, partes[0].Length)].ToUpperInvariant();
        }

        return $"{partes[0][0]}{partes[^1][0]}".ToUpperInvariant();
    }
}
