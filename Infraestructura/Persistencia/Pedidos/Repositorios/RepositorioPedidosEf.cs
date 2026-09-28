using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Aplicacion.Pedidos.Dtos;
using VoltajeModa.Core.Aplicacion.Pedidos.Interfaces;
using VoltajeModa.Core.Dominio.Comun.ValueObjects;
using VoltajeModa.Core.Dominio.Pedidos;
using VoltajeModa.Data;
using VoltajeModa.Infraestructura.Persistencia.Pedidos.Mapeadores;
using EfModels = VoltajeModa.Models;

namespace VoltajeModa.Infraestructura.Persistencia.Pedidos.Repositorios;

public class RepositorioPedidosEf : IRepositorioPedidos
{
    private readonly ApplicationDbContext _context;

    public RepositorioPedidosEf(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> ObtenerPorIdAsync(int id, CancellationToken ct)
    {
        var entidad = await Consulta().FirstOrDefaultAsync(p => p.Id == id, ct);
        return entidad is null ? null : PedidoMapeador.ADominio(entidad);
    }

    public async Task<IReadOnlyList<Pedido>> ListarPorTitularAsync(Titular titular, CancellationToken ct)
    {
        var consulta = titular.EsUsuario
            ? Consulta().Where(p => p.UsuarioId == titular.UsuarioId)
            : Consulta().Where(p => p.AnonimoId == titular.AnonimoId);

        var entidades = await consulta.OrderByDescending(p => p.Fecha).ToListAsync(ct);
        return entidades.Select(PedidoMapeador.ADominio).ToList();
    }

    public async Task<(IReadOnlyList<Pedido> Items, int Total)> ListarTodosAsync(FiltroPedidos filtro, CancellationToken ct)
    {
        var consulta = Consulta();

        if (filtro.Estado is EstadoPedido estado)
        {
            var estadoEf = (EfModels.EstadoPedido)(int)estado;
            consulta = consulta.Where(p => p.Estado == estadoEf);
        }

        if (filtro.Desde is DateTime desde)
        {
            consulta = consulta.Where(p => p.Fecha >= desde);
        }

        if (filtro.Hasta is DateTime hasta)
        {
            consulta = consulta.Where(p => p.Fecha <= hasta);
        }

        var total = await consulta.CountAsync(ct);
        var pagina = Math.Max(filtro.Pagina, 1);
        var tamanoPagina = Math.Clamp(filtro.TamanoPagina, 1, 100);

        var entidades = await consulta
            .OrderByDescending(p => p.Fecha)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(ct);

        return (entidades.Select(PedidoMapeador.ADominio).ToList(), total);
    }

    public Task AgregarAsync(Pedido pedido, CancellationToken ct)
    {
        var entidad = PedidoMapeador.AEntidadNueva(pedido);
        _context.Pedidos.Add(entidad);
        SuscribirAsignacionDeId(pedido, entidad);
        return Task.CompletedTask;
    }

    public async Task ActualizarAsync(Pedido pedido, CancellationToken ct)
    {
        var entidad = await Consulta().FirstOrDefaultAsync(p => p.Id == pedido.Id, ct);
        if (entidad is null)
        {
            return;
        }

        PedidoMapeador.VolcarEnEntidad(pedido, entidad);
    }

    private IQueryable<EfModels.Pedido> Consulta() => _context.Pedidos.Include(p => p.Items);

    private void SuscribirAsignacionDeId(Pedido pedido, EfModels.Pedido entidad)
    {
        void Handler(object? sender, SavedChangesEventArgs e)
        {
            pedido.AsignarId(entidad.Id);
            _context.SavedChanges -= Handler;
        }

        _context.SavedChanges += Handler;
    }
}
