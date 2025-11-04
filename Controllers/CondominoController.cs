using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoElvis2.Context;
using TrabalhoElvis2.Models;

namespace TrabalhoElvis2.Controllers
{
    public class CondominoController : Controller
    {
        private readonly LoginContext _ctx;
        public CondominoController(LoginContext ctx) => _ctx = ctx;

        // === LISTAR ===
        public async Task<IActionResult> Index()
        {
            var lista = await _ctx.Condominos
                .Include(c => c.Imoveis)
                .OrderBy(c => c.NomeCompleto)
                .ToListAsync();
            return View(lista);
        }

        // === CADASTRAR ===
        [HttpPost]
        public async Task<IActionResult> Cadastrar(Condomino model)
        {
            if (!ModelState.IsValid)
                return View("Index", await _ctx.Condominos.ToListAsync());

            _ctx.Condominos.Add(model);
            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
