using VoltajeModa.Core.Aplicacion.Carritos.Interfaces;
using VoltajeModa.Core.Aplicacion.Comun.Interfaces;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Inventario.Excepciones;
using VoltajeModa.Core.Dominio.Pedidos;
using VoltajeModa.Core.Dominio.Pedidos.Excepciones;
using StockInsuficienteException = VoltajeModa.Core.Dominio.Pedidos.Excepciones.StockInsuficienteException;

namespace VoltajeModa.Core.Aplicacion.Pedidos.CasosDeUso;

// Flujo central del negocio: valida el carrito y la dirección, toma un snapshot del precio
// vigente de cada línea, descuenta stock, crea el pedido y vacía el carrito. Todo en un único
// IUnidadDeTrabajo.GuardarCambiosAsync() al final para que sea atómico (si falla el stock de
// cualquier línea, nada de lo anterior queda persistido).
public class CrearPedidoDesdeCarritoCasoDeUso
{
    private readonly ILectorCarritoParaCheckout _lectorCarrito;
    private readonly IRepositorioDirecciones _direcciones;
    private readonly IConsultaVarianteCheckout _consultaVariante;
    private readonly IAjustadorStock _ajustadorStock;
    private readonly IRepositorioPedidos _pedidos;
    private readonly IRepositorioCarritos _carritos;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public CrearPedidoDesdeCarritoCasoDeUso(
        ILectorCarritoParaCheckout lectorCarrito, IRepositorioDirecciones direcciones, IConsultaVarianteCheckout consultaVariante,
        IAjustadorStock ajustadorStock, IRepositorioPedidos pedidos, IRepositorioCarritos carritos, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _lectorCarrito = lectorCarrito;
        _direcciones = direcciones;
        _consultaVariante = consultaVariante;
        _ajustadorStock = ajustadorStock;
        _pedidos = pedidos;
        _carritos = carritos;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<Pedido> EjecutarAsync(Titular titular, int direccionId, CancellationToken ct)
    {
        var lineasCarrito = await _lectorCarrito.ObtenerLineasAsync(titular, ct);
        if (lineasCarrito.Count == 0)
        {
            throw new PedidoVacioException();
        }

        var direccion = await _direcciones.ObtenerPorIdAsync(direccionId, ct) ?? throw new DireccionNoEncontradaException(direccionId);
        if (!direccion.Titular.Coincide(titular.UsuarioId, titular.AnonimoId))
        {
            throw new DireccionNoEncontradaException(direccionId);
        }

        var lineasConPrecio = new List<(int ProductoVarianteId, int Cantidad, decimal PrecioUnitario)>();
        foreach (var linea in lineasCarrito)
        {
            var (existe, precioUnitario) = await _consultaVariante.ObtenerAsync(linea.ProductoVarianteId, ct);
            if (!existe)
            {
                throw new VarianteNoEncontradaException(linea.ProductoVarianteId);
            }

            lineasConPrecio.Add((linea.ProductoVarianteId, linea.Cantidad, precioUnitario));
        }

        var pedido = Pedido.CrearDesdeCarrito(titular, direccionId, lineasConPrecio);

        foreach (var linea in lineasConPrecio)
        {
            var resultado = await _ajustadorStock.DescontarPorVentaAsync(
                linea.ProductoVarianteId, linea.Cantidad, "Venta - checkout de pedido", titular.UsuarioId, ct);

            if (!resultado.Exito)
            {
                throw new StockInsuficienteException(resultado.Error ?? "Stock insuficiente para completar el pedido.");
            }
        }

        await _pedidos.AgregarAsync(pedido, ct);

        var carrito = titular.EsUsuario
            ? await _carritos.ObtenerPorUsuarioAsync(titular.UsuarioId!, ct)
            : await _carritos.ObtenerPorAnonimoAsync(titular.AnonimoId!, ct);

        if (carrito is not null)
        {
            carrito.Vaciar();
            await _carritos.ActualizarAsync(carrito, ct);
        }

        await _unidadDeTrabajo.GuardarCambiosAsync(ct);

        return pedido;
    }
}
