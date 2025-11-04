using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabalhoElvis2.Context;
using TrabalhoElvis2.Models;

namespace TrabalhoElvis2.Controllers
{
    public class ImovelController : Controller
    {
        private readonly LoginContext _ctx;
        public ImovelController(LoginContext ctx)
        {
            _ctx = ctx;
        }

        // ==== LISTAR ====
        public async Task<IActionResult> Index()
        {
            // Verifica se o usuário é Administrador
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");
            if (tipoUsuario != "Administrador")
                return RedirectToAction("Login", "Usuario");

            var lista = await _ctx.Imoveis
                .Include(i => i.Condomino)
                .OrderBy(i => i.Codigo)
                .ToListAsync();

            // ViewBag para o modal de cadastro
            ViewBag.Condominos = new SelectList(
                _ctx.Condominos.OrderBy(c => c.NomeCompleto).ToList(),
                "Id",
                "NomeCompleto"
            );

            return View(lista);
        }

        // ==== CADASTRAR ====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Imovel imovel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Condominos = new SelectList(_ctx.Condominos.OrderBy(c => c.NomeCompleto).ToList(), "Id", "NomeCompleto");
                var lista = await _ctx.Imoveis.Include(i => i.Condomino).ToListAsync();
                return View("Index", lista);
            }

            _ctx.Imoveis.Add(imovel);
            await _ctx.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Imóvel cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // ==== EDITAR ====
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var imovel = await _ctx.Imoveis.FindAsync(id);
            if (imovel == null)
                return NotFound();

            ViewBag.Condominos = new SelectList(
                _ctx.Condominos.OrderBy(c => c.NomeCompleto).ToList(),
                "Id",
                "NomeCompleto",
                imovel.CondominoId
            );

            return View(imovel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Imovel imovel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Condominos = new SelectList(_ctx.Condominos.OrderBy(c => c.NomeCompleto).ToList(), "Id", "NomeCompleto", imovel.CondominoId);
                return View(imovel);
            }

            _ctx.Imoveis.Update(imovel);
            await _ctx.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Imóvel atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // ==== EXCLUIR ====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            var imovel = await _ctx.Imoveis.FindAsync(id);
            if (imovel == null)
                return NotFound();

            _ctx.Imoveis.Remove(imovel);
            await _ctx.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Imóvel excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
