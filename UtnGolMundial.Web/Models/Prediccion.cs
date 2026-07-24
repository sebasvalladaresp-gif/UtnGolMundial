namespace UtnGolMundial.Web.Models;

public enum EstadoPrediccion { Pendiente, Ganada, Perdida }

public class Prediccion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int PartidoId { get; set; }
    public ResultadoPartido ResultadoPredicho { get; set; }
    public decimal Monto { get; set; }
    public decimal Cuota { get; set; } = 2.00m;
    public EstadoPrediccion Estado { get; set; } = EstadoPrediccion.Pendiente;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
