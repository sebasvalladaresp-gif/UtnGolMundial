using Microsoft.EntityFrameworkCore;
using UtnGolMundial.Web.Data;
using UtnGolMundial.Web.Models;

namespace UtnGolMundial.Web.Services;

public class PartidoService
{
    private readonly ApplicationDbContext _context;
    private readonly PrediccionService _prediccionService;

    public PartidoService(ApplicationDbContext context, PrediccionService prediccionService)
    {
        _context = context;
        _prediccionService = prediccionService;
    }

    public List<Partido> Listar() =>
        _context.Partidos.OrderBy(p => p.FechaHora).ToList();

    public Partido ObtenerPorId(int id) =>
        _context.Partidos.Find(id) ?? throw new NegocioException("Partido no encontrado");

    public Partido Crear(string grupo, string local, string visitante, DateTime fechaHora)
    {
        var p = new Partido
        {
            Grupo = grupo,
            EquipoLocal = local,
            EquipoVisitante = visitante,
            FechaHora = fechaHora,
            Estado = EstadoPartido.Programado
        };

        _context.Partidos.Add(p);
        _context.SaveChanges(); // Guarda y asigna el Id automáticamente
        return p;
    }

    // RF12/RF19: registra el resultado oficial y liquida automaticamente todas las predicciones pendientes
    public Partido RegistrarResultado(int partidoId, int golesLocal, int golesVisitante)
    {
        var p = ObtenerPorId(partidoId);
        p.GolesLocal = golesLocal;
        p.GolesVisitante = golesVisitante;
        p.Estado = EstadoPartido.Finalizado;
        p.ResultadoOficial = golesLocal > golesVisitante ? ResultadoPartido.Local
            : golesLocal < golesVisitante ? ResultadoPartido.Visitante
            : ResultadoPartido.Empate;

        _context.SaveChanges(); // Guarda los cambios del partido en la base de datos

        _prediccionService.Liquidar(p.Id, p.ResultadoOficial.Value);
        return p;
    }
}