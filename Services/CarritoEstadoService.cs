namespace VoltajeModa.Services;

// Servicio scoped (por circuito) que avisa a otros componentes -como el contador del ícono del
// carrito en la navbar- cuando el contenido del carrito cambia, ya que ni CarritoService ni las
// páginas que mutan CarritoItems mantienen estado compartido ni levantan eventos por su cuenta.
public class CarritoEstadoService
{
    public event Action? Cambio;

    public void Notificar() => Cambio?.Invoke();
}
