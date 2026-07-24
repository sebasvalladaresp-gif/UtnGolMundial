using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Data;

namespace UtnGolMundial.Web.Controllers.Api;

[ApiController]
[Route("api/ranking")]
[AllowAnonymous]
public class RankingApiController : ControllerBase
{
    private readonly ApplicationDbContext _store;

    public RankingApiController(ApplicationDbContext store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult Ranking()
    {
        var ranking = _store.Usuarios
            .Select(u => new
            {
                u.Nombre,
                Saldo = _store.Billeteras
                    .Where(b => b.UsuarioId == u.Id)
                    .Select(b => (decimal?)b.Saldo)
                    .FirstOrDefault() ?? 0m
            })
            .OrderByDescending(x => x.Saldo)
            .Take(50)
            .ToList();

        return Ok(ranking);
    }
}