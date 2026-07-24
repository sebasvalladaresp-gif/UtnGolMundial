using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Controllers;
using UtnGolMundial.Web.Models;
using UtnGolMundial.Web.Services;

namespace UtnGolMundial.Web.Controllers.Api;

public record CrearPrediccionDto(int PartidoId, ResultadoPartido ResultadoPredicho, decimal Monto);

[ApiController]
[Route("api/predicciones")]
[Authorize]
public class PrediccionesApiController : ControllerBase
{
    private readonly PrediccionService _prediccionService;

    public PrediccionesApiController(PrediccionService prediccionService)
    {
        _prediccionService = prediccionService;
    }

    [HttpGet("mias")]
    public IActionResult Mias() => Ok(_prediccionService.ListarPorUsuario(User.UsuarioId()));

    [HttpPost]
    public IActionResult Crear(CrearPrediccionDto dto)
    {
        try
        {
            var p = _prediccionService.Crear(User.UsuarioId(), dto.PartidoId, dto.ResultadoPredicho, dto.Monto);
            return Created($"/api/predicciones/{p.Id}", p);
        }
        catch (NegocioException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
