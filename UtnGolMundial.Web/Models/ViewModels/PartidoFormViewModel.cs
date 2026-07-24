namespace UtnGolMundial.Web.Models.ViewModels;

public class PartidoFormViewModel
{
    public string Grupo { get; set; } = "";
    public string EquipoLocal { get; set; } = "";
    public string EquipoVisitante { get; set; } = "";
    public DateTime FechaHora { get; set; } = DateTime.Now.AddDays(1);
}
