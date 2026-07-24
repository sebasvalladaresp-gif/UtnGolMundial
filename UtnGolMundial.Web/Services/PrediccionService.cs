using UtnGolMundial.Web.Data;
using UtnGolMundial.Web.Models;

namespace UtnGolMundial.Web.Services;

public class PrediccionService
{
    private static readonly object _lock = new();
    private readonly ApplicationDbContext _context;

    public PrediccionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Prediccion Crear(int usuarioId, int partidoId, ResultadoPartido resultadoPredicho, decimal monto)
    {
        if (monto <= 0)
            throw new NegocioException("El monto debe ser mayor a cero");

        var partido = _context.Partidos.Find(partidoId)
            ?? throw new NegocioException("Partido no encontrado");

        if (DateTime.Now >= partido.FechaHora)
            throw new NegocioException("Ya no se puede predecir: el partido ya inició o finalizó");

        bool yaExiste = _context.Predicciones.Any(pr => pr.UsuarioId == usuarioId && pr.PartidoId == partidoId);
        if (yaExiste)
            throw new NegocioException("Ya registraste una predicción para este partido");

        lock (_lock)
        {
            var billetera = _context.Billeteras.FirstOrDefault(b => b.UsuarioId == usuarioId)
                ?? throw new NegocioException("Billetera no encontrada");

            if (billetera.Saldo < monto)
                throw new NegocioException("Saldo insuficiente para esta predicción");

            billetera.Saldo -= monto;

            _context.Transacciones.Add(new Transaccion
            {
                UsuarioId = usuarioId,
                Tipo = TipoTransaccion.Prediccion,
                Monto = -monto,
                Detalle = $"Predicción: {partido.EquipoLocal} vs {partido.EquipoVisitante}",
                Fecha = DateTime.UtcNow
            });

            var prediccion = new Prediccion
            {
                UsuarioId = usuarioId,
                PartidoId = partidoId,
                ResultadoPredicho = resultadoPredicho,
                Monto = monto,
                Cuota = 2.00m,
                Estado = EstadoPrediccion.Pendiente,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Predicciones.Add(prediccion);
            _context.SaveChanges();

            return prediccion;
        }
    }

    public List<Prediccion> ListarPorUsuario(int usuarioId) =>
        _context.Predicciones
            .Where(p => p.UsuarioId == usuarioId)
            .OrderByDescending(p => p.FechaCreacion)
            .ToList();

    public void Liquidar(int partidoId, ResultadoPartido resultadoOficial)
    {
        lock (_lock)
        {
            var pendientes = _context.Predicciones
                .Where(p => p.PartidoId == partidoId && p.Estado == EstadoPrediccion.Pendiente)
                .ToList();

            foreach (var p in pendientes)
            {
                var billetera = _context.Billeteras.FirstOrDefault(b => b.UsuarioId == p.UsuarioId);
                if (billetera == null) continue;

                if (p.ResultadoPredicho == resultadoOficial)
                {
                    var premio = p.Monto * p.Cuota;
                    billetera.Saldo += premio;

                    _context.Transacciones.Add(new Transaccion
                    {
                        UsuarioId = p.UsuarioId,
                        Tipo = TipoTransaccion.Premio,
                        Monto = premio,
                        Detalle = $"Premio partido #{partidoId}",
                        Fecha = DateTime.UtcNow
                    });

                    p.Estado = EstadoPrediccion.Ganada;
                }
                else
                {
                    p.Estado = EstadoPrediccion.Perdida;
                }
            }

            _context.SaveChanges();
        }
    }
}