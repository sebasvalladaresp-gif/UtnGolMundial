using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Controllers;
using UtnGolMundial.Web.Services;

namespace UtnGolMundial.Web.Controllers.Api;

[ApiController]
[Route("api/billetera")]
[Authorize]
public class BilleteraApiController : ControllerBase
{
    private readonly BilleteraService _billeteraService;

    public BilleteraApiController(BilleteraService billeteraService)
    {
        _billeteraService = billeteraService;
    }

    [HttpGet("saldo")]
    public IActionResult Saldo() => Ok(_billeteraService.ObtenerSaldo(User.UsuarioId()));

    [HttpGet("historial")]
    public IActionResult Historial() => Ok(_billeteraService.Historial(User.UsuarioId()));
}
