namespace UtnGolMundial.Web.Models;

public enum Rol { Administrador, Usuario, Invitado }

public class Usuario
{
    public int Id { get; set; }
    public string Correo { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Nombre { get; set; } = "";
    public Rol Rol { get; set; } = Rol.Usuario;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
