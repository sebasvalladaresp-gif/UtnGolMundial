using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Models.ViewModels;
using UtnGolMundial.Web.Services;

namespace UtnGolMundial.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly PartidoService _partidoService;
    private readonly PrediccionService _prediccionService;
    private readonly BilleteraService _billeteraService;

    public HomeController(PartidoService partidoService, PrediccionService prediccionService, BilleteraService billeteraService)
    {
        _partidoService = partidoService;
        _prediccionService = prediccionService;
        _billeteraService = billeteraService;
    }

    public IActionResult Index()
    {
        _billeteraService.ReclamarBonoDiarioSiAplica(User.UsuarioId());

        ViewBag.Partidos = _partidoService.Listar();
        ViewBag.MisPredicciones = _prediccionService.ListarPorUsuario(User.UsuarioId());
        ViewBag.Saldo = _billeteraService.ObtenerSaldo(User.UsuarioId()).Saldo;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Predecir(PrediccionFormViewModel modelo)
    {
        try
        {
            _prediccionService.Crear(User.UsuarioId(), modelo.PartidoId, modelo.ResultadoPredicho, modelo.Monto);
            TempData["Mensaje"] = "¡Predicción registrada!";
        }
        catch (NegocioException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Index");
    }

    [AllowAnonymous]
    public IActionResult Error() => View();
}