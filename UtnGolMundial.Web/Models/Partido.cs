namespace UtnGolMundial.Web.Models;

public enum EstadoPartido { Programado, EnJuego, Finalizado }
public enum ResultadoPartido { Local, Empate, Visitante }

public class Partido
{
    public int Id { get; set; }
    public string Grupo { get; set; } = "";
    public string EquipoLocal { get; set; } = "";
    public string EquipoVisitante { get; set; } = "";
    public DateTime FechaHora { get; set; }
    public EstadoPartido Estado { get; set; } = EstadoPartido.Programado;

    public int? GolesLocal { get; set; }
    public int? GolesVisitante { get; set; }
    public ResultadoPartido? ResultadoOficial { get; set; }
}
