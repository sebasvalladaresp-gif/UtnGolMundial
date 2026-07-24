using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Services;

namespace UtnGolMundial.Web.Controllers.Api;

public record ResultadoDto(int GolesLocal, int GolesVisitante);

[ApiController]
[Route("api/partidos")]
public class PartidosApiController : ControllerBase
{
    private readonly PartidoService _partidoService;

    public PartidosApiController(PartidoService partidoService)
    {
        _partidoService = partidoService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Listar() => Ok(_partidoService.Listar());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult ObtenerPorId(int id)
    {
        try { return Ok(_partidoService.ObtenerPorId(id)); }
        catch (NegocioException ex) { return NotFound(new { error = ex.Message }); }
    }

    [HttpPut("{id}/resultado")]
    [Authorize(Roles = "Administrador")]
    public IActionResult RegistrarResultado(int id, ResultadoDto dto)
    {
        try { return Ok(_partidoService.RegistrarResultado(id, dto.GolesLocal, dto.GolesVisitante)); }
        catch (NegocioException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
