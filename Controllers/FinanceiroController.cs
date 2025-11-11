using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using TrabalhoElvis2.Context;
using TrabalhoElvis2.Models;

namespace TrabalhoElvis2.Controllers
{
    public class FinanceiroController : Controller
    {
        private readonly LoginContext _context;
        private readonly IWebHostEnvironment _env;

        public FinanceiroController(LoginContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ========== ADMINISTRADOR ==========
        [HttpGet]
        public async Task<IActionResult> Administrador(string filtro = "Todos")
        {
            // Atualiza status automaticamente
            var boletos = await _context.Boletos
                .Include(b => b.Contrato)
                    .ThenInclude(c => c.Imovel)
                .Include(b => b.Contrato)
                    .ThenInclude(c => c.Condomino)
                .ToListAsync();

            foreach (var b in boletos)
            {
                if (b.Status == "Pendente" && b.Vencimento < DateTime.Today)
                    b.Status = "Vencido";
            }
            await _context.SaveChangesAsync();

            // Filtro
            if (filtro != "Todos")
                boletos = boletos.Where(b => b.Status == filtro).ToList();

            // Estatísticas
            ViewBag.ReceitaPrevista = boletos.Sum(b => b.Valor);
            ViewBag.ReceitaRecebida = boletos.Where(b => b.Status == "Pago").Sum(b => b.Valor);
            ViewBag.Inadimplencia = boletos.Where(b => b.Status == "Vencido").Sum(b => b.Valor);

            return View("Administrador", boletos);
        }

        // ========== GERAR QR CODE PIX ==========
        [HttpPost]
        public async Task<IActionResult> GerarQrCode(int id)
        {
            var boleto = await _context.Boletos
                .Include(b => b.Contrato)
                    .ThenInclude(c => c.Condomino)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (boleto == null)
                return NotFound();

            // Cria o texto do QR Code (pode ser substituído por integração real de PIX)
            string textoPix = $"Pagamento condomínio - {boleto.Contrato.Condomino.NomeCompleto} - R$ {boleto.Valor:N2}";

            string pasta = Path.Combine(_env.WebRootPath, "qrcodes");
            Directory.CreateDirectory(pasta);

            string caminho = Path.Combine(pasta, $"qrcode_{boleto.Id}.png");

            using (QRCodeGenerator gen = new QRCodeGenerator())
            {
                var data = gen.CreateQrCode(textoPix, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qr = new QRCode(data))
                using (Bitmap bmp = qr.GetGraphic(20))
                    bmp.Save(caminho, ImageFormat.Png);
            }

            boleto.QrCodePix = $"/qrcodes/qrcode_{boleto.Id}.png";
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "QR Code gerado com sucesso!";
            return RedirectToAction("Administrador");
        }

        // ========== CONFIRMAR PAGAMENTO ==========
        [HttpPost]
        public async Task<IActionResult> ConfirmarPagamento(int id)
        {
            var boleto = await _context.Boletos.FindAsync(id);
            if (boleto == null)
                return NotFound();

            boleto.Status = "Pago";
            boleto.Pagamento = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Pagamento confirmado com sucesso!";
            return RedirectToAction("Administrador");
        }
    }
}
