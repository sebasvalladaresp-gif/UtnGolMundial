using System.Security.Claims;

namespace UtnGolMundial.Web.Controllers;

public static class ClaimsPrincipalExtensions
{
    public static int UsuarioId(this ClaimsPrincipal usuario) =>
        int.Parse(usuario.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
}
