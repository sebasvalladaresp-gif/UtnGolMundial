using UtnGolMundial.Web.Data;
using UtnGolMundial.Web.Models;

namespace UtnGolMundial.Web.Services;

public class BilleteraService
{
    private static readonly object _lock = new();
    private const decimal BonoDiario = 1.00m;
    private readonly ApplicationDbContext _context;

    public BilleteraService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Billetera ObtenerSaldo(int usuarioId)
    {
        var billetera = _context.Billeteras.FirstOrDefault(b => b.UsuarioId == usuarioId);
        return billetera ?? throw new NegocioException("Billetera no encontrada");
    }

    public List<Transaccion> Historial(int usuarioId)
    {
        return _context.Transacciones
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.Fecha)
            .ToList();
    }

    // RF20: bono diario anti-bancarrota si el saldo llega a 0, una vez por dia
    public Billetera ReclamarBonoDiarioSiAplica(int usuarioId)
    {
        lock (_lock)
        {
            var billetera = ObtenerSaldo(usuarioId);
            var hoy = DateTime.UtcNow.Date;

            // Verificamos en la base de datos si ya recibió un bono diario el día de hoy
            bool yaRecibioHoy = _context.Transacciones.Any(t =>
                t.UsuarioId == usuarioId &&
                t.Tipo == TipoTransaccion.BonoDiario &&
                t.Fecha.Date == hoy);

            if (billetera.Saldo == 0 && !yaRecibioHoy)
            {
                billetera.Saldo += BonoDiario;

                var transaccion = new Transaccion
                {
                    UsuarioId = usuarioId,
                    Tipo = TipoTransaccion.BonoDiario,
                    Monto = BonoDiario,
                    Detalle = "Bono diario anti-bancarrota",
                    Fecha = DateTime.UtcNow
                };

                _context.Transacciones.Add(transaccion);
                _context.SaveChanges();
            }

            return billetera;
        }
    }
}