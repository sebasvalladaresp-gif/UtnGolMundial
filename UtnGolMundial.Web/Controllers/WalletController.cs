using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Services;

namespace UtnGolMundial.Web.Controllers;

[Authorize]
public class WalletController : Controller
{
    private readonly BilleteraService _billeteraService;

    public WalletController(BilleteraService billeteraService)
    {
        _billeteraService = billeteraService;
    }

    public IActionResult Index()
    {
        ViewBag.Saldo = _billeteraService.ObtenerSaldo(User.UsuarioId()).Saldo;
        ViewBag.Historial = _billeteraService.Historial(User.UsuarioId());
        return View();
    }
}
