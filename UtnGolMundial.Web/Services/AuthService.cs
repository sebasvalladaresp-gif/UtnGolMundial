using UtnGolMundial.Web.Data;
using UtnGolMundial.Web.Models;

namespace UtnGolMundial.Web.Services;

public class AuthService
{
    private const decimal BonoBienvenida = 10.00m;
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Usuario Registrar(string correo, string password, string nombre)
    {
        if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(nombre))
            throw new NegocioException("Correo, contraseña y nombre son obligatorios");

        // Verificamos si ya existe el correo usando EF Core
        if (_context.Usuarios.Any(u => u.Correo.ToLower() == correo.ToLower()))
            throw new NegocioException("Ya existe una cuenta con ese correo");

        var usuario = new Usuario
        {
            Correo = correo,
            Nombre = nombre,
            PasswordHash = PasswordHasher.Hash(password),
            Rol = Rol.Usuario
        };

        _context.Usuarios.Add(usuario);
        _context.SaveChanges(); // Guardamos primero para que se genere el Id del usuario

        var billetera = new Billetera
        {
            UsuarioId = usuario.Id,
            Saldo = BonoBienvenida
        };
        _context.Billeteras.Add(billetera);

        var transaccion = new Transaccion
        {
            UsuarioId = usuario.Id,
            Tipo = TipoTransaccion.BonoBienvenida,
            Monto = BonoBienvenida,
            Detalle = "Bono de bienvenida al registrarse"
        };
        _context.Transacciones.Add(transaccion);

        _context.SaveChanges(); // Guardamos los cambios de la billetera y la transacción

        return usuario;
    }

    public Usuario Login(string correo, string password)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo.ToLower() == correo.ToLower())
            ?? throw new NegocioException("Correo o contraseña incorrectos");

        if (!PasswordHasher.Verify(password, usuario.PasswordHash))
            throw new NegocioException("Correo o contraseña incorrectos");

        return usuario;
    }
}