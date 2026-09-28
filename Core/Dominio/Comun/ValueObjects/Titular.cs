namespace VoltajeModa.Core.Dominio.Comun.ValueObjects;

public sealed record Titular
{
    private Titular(string? usuarioId, string? anonimoId)
    {
        UsuarioId = usuarioId;
        AnonimoId = anonimoId;
    }

    public string? UsuarioId { get; }
    public string? AnonimoId { get; }
    public bool EsUsuario => UsuarioId is not null;

    public static Titular DeUsuario(string usuarioId)
    {
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            throw new ArgumentException("El id de usuario no puede estar vacío.", nameof(usuarioId));
        }

        return new Titular(usuarioId, null);
    }

    public static Titular DeInvitado(string anonimoId)
    {
        if (string.IsNullOrWhiteSpace(anonimoId))
        {
            throw new ArgumentException("El id de invitado no puede estar vacío.", nameof(anonimoId));
        }

        return new Titular(null, anonimoId);
    }

    public bool Coincide(string? usuarioId, string? anonimoId)
    {
        return EsUsuario ? UsuarioId == usuarioId : AnonimoId == anonimoId;
    }
}
