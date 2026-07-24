using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Models;
using UtnGolMundial.Web.Services;

namespace UtnGolMundial.Web.Controllers.Api;

public record RegistroDto(string Correo, string Password, string Nombre);
public record LoginDto(string Correo, string Password);

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthApiController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthApiController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registrar(RegistroDto dto)
    {
        try
        {
            var usuario = _authService.Registrar(dto.Correo, dto.Password, dto.Nombre);
            await IniciarSesionAsync(usuario);
            return Created($"/api/usuarios/{usuario.Id}", new { usuario.Id, usuario.Nombre, usuario.Correo, Rol = usuario.Rol.ToString() });
        }
        catch (NegocioException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var usuario = _authService.Login(dto.Correo, dto.Password);
            await IniciarSesionAsync(usuario);
            return Ok(new { usuario.Id, usuario.Nombre, usuario.Correo, Rol = usuario.Rol.ToString() });
        }
        catch (NegocioException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    private async Task IniciarSesionAsync(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nombre),
            new(ClaimTypes.Email, usuario.Correo),
            new(ClaimTypes.Role, usuario.Rol.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}
