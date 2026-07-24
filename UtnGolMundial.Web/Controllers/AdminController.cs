using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Data;
using UtnGolMundial.Web.Models;
using UtnGolMundial.Web.Models.ViewModels;
using UtnGolMundial.Web.Services;

namespace UtnGolMundial.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly PartidoService _partidoService;
    private readonly ApplicationDbContext _store;

    public AdminController(PartidoService partidoService, ApplicationDbContext store)
    {
        _partidoService = partidoService;
        _store = store;
    }

    public IActionResult Partidos()
    {
        ViewBag.Partidos = _partidoService.Listar();
        return View();
    }

    public IActionResult Usuarios()
    {
        var usuarios = _store.Usuarios
            .OrderBy(u => u.Nombre)
            .Select(u => new
            {
                u.Id,
                u.Nombre,
                u.Correo,
                u.Rol,
                Saldo = _store.Billeteras
                    .Where(b => b.UsuarioId == u.Id)
                    .Select(b => (decimal?)b.Saldo)
                    .FirstOrDefault() ?? 0m
            }).ToList();

        ViewBag.Usuarios = usuarios;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarRol(int usuarioId, Rol nuevoRol)
    {
        var usuario = _store.Usuarios.Find(usuarioId);
        if (usuario != null)
        {
            usuario.Rol = nuevoRol;
            _store.SaveChanges();
            TempData["Mensaje"] = $"Rol de {usuario.Nombre} actualizado a {nuevoRol}.";
        }
        else
        {
            TempData["Error"] = "Usuario no encontrado.";
        }
        return RedirectToAction("Usuarios");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearPartido(PartidoFormViewModel modelo)
    {
        try
        {
            _partidoService.Crear(modelo.Grupo, modelo.EquipoLocal, modelo.EquipoVisitante, modelo.FechaHora);
            TempData["Mensaje"] = "Partido creado.";
        }
        catch (NegocioException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Partidos");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RegistrarResultado(ResultadoFormViewModel modelo)
    {
        try
        {
            _partidoService.RegistrarResultado(modelo.PartidoId, modelo.GolesLocal, modelo.GolesVisitante);
            TempData["Mensaje"] = "Resultado registrado y predicciones liquidadas.";
        }
        catch (NegocioException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Partidos");
    }
}