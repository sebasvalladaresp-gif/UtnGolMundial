using System.ComponentModel.DataAnnotations;

namespace UtnGolMundial.Web.Models;

public class Billetera
{
    [Key]
    public int UsuarioId { get; set; }
    public decimal Saldo { get; set; } = 0m;
}
