using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Models;
using UtnGolMundial.Web.Models.ViewModels;
using UtnGolMundial.Web.Services;

namespace UtnGolMundial.Web.Controllers;

public class AccountController : Controller
{
    private readonly AuthService _authService;

    public AccountController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        try
        {
            var usuario = _authService.Login(modelo.Correo, modelo.Password);
            await IniciarSesionAsync(usuario);
            return RedirectToAction("Index", "Home");
        }
        catch (NegocioException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(modelo);
        }
    }

    [HttpGet]
    public IActionResult Registro() => View(new RegistroViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        try
        {
            var usuario = _authService.Registrar(modelo.Correo, modelo.Password, modelo.Nombre);
            await IniciarSesionAsync(usuario);
            TempData["Mensaje"] = "¡Cuenta creada! Te regalamos 10 UTNGolCoin de bienvenida.";
            return RedirectToAction("Index", "Home");
        }
        catch (NegocioException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(modelo);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult AccesoDenegado() => View();

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
