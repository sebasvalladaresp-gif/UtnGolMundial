using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UtnGolMundial.Web.Data;

namespace UtnGolMundial.Web.Controllers;

[Authorize]
public class RankingController : Controller
{
    private readonly ApplicationDbContext _store;

    public RankingController(ApplicationDbContext store)
    {
        _store = store;
    }

    public IActionResult Index()
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

        ViewBag.Ranking = ranking;
        return View();
    }
}