using VoltajeModa.Core.Dominio.Pedidos;

namespace VoltajeModa.Core.Aplicacion.Pedidos.Dtos;

public record FiltroPedidos(EstadoPedido? Estado, DateTime? Desde, DateTime? Hasta, int Pagina = 1, int TamanoPagina = 20);
