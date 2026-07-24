namespace UtnGolMundial.Web.Models;

public enum TipoTransaccion { BonoBienvenida, Prediccion, Premio, BonoDiario }

public class Transaccion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public TipoTransaccion Tipo { get; set; }
    public decimal Monto { get; set; }
    public string Detalle { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
