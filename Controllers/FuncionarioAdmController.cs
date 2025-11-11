using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using TrabalhoElvis2.Context;
using TrabalhoElvis2.Models;

namespace TrabalhoElvis2.Controllers
{
    public class FinanceiroAdmController : Controller
    {
        private readonly LoginContext _context;
        private readonly IWebHostEnvironment _env;

        public FinanceiroAdmController(LoginContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Administrador(string filtro = "Todos")
        {
            var boletos = await _context.Boletos
                .Include(b => b.Contrato)
                    .ThenInclude(c => c.Condomino)
                .Include(b => b.Contrato)
                    .ThenInclude(c => c.Imovel)
                .ToListAsync();

            // Aplica filtro
            if (filtro != "Todos")
                boletos = boletos.Where(b => b.Status == filtro).ToList();

            // Cálculos
            ViewBag.ReceitaPrevista = boletos.Sum(b => b.Valor);
            ViewBag.ReceitaRecebida = boletos.Where(b => b.Status == "Pago").Sum(b => b.Valor);
            ViewBag.Inadimplencia = boletos.Where(b => b.Status == "Vencido" || b.Status == "Pendente")
                                           .Sum(b => b.Valor);

            return View(boletos);
        }

        [HttpPost]
        public async Task<IActionResult> GerarQrCode(int id)
        {
            var boleto = await _context.Boletos
                .Include(b => b.Contrato)
                .ThenInclude(c => c.Condomino)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (boleto == null)
                return NotFound();

            // Simula código PIX (geração simples)
            string chavePix = "chavepix@vizinapp.com";
            string payload = $"pix://{chavePix}?valor={boleto.Valor}&nome={boleto.Contrato.Condomino.NomeCompleto}";

            using (var qrGen = new QRCodeGenerator())
            {
                var data = qrGen.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCode(data);
                using var bitmap = qrCode.GetGraphic(20);

                string path = Path.Combine(_env.WebRootPath, "qrcodes");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string filePath = Path.Combine(path, $"qrcode_{id}.png");
                bitmap.Save(filePath, ImageFormat.Png);

                boleto.QrCodePix = $"/qrcodes/qrcode_{id}.png";
                await _context.SaveChangesAsync();
            }

            TempData["MensagemSucesso"] = "QR Code gerado com sucesso!";
            return RedirectToAction("Administrador");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPagamento(int id)
        {
            var boleto = await _context.Boletos.FindAsync(id);
            if (boleto == null) return NotFound();

            boleto.Status = "Pago";
            boleto.Pagamento = DateTime.Now;

            _context.Update(boleto);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Pagamento confirmado com sucesso!";
            return RedirectToAction("Administrador");
        }

        [HttpGet]
        public async Task<IActionResult> CriarBoleto()
        {
            // Carrega todos os contratos com nome e imóvel
            var contratos = await _context.Contratos
                .Include(c => c.Imovel)
                .Include(c => c.Condomino)
                .Select(c => new
                {
                    c.Id,
                    Descricao = $"{c.Imovel.Codigo} - {c.Condomino.NomeCompleto}"
                })
                .ToListAsync();

            ViewBag.Contratos = contratos;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarBoleto(int contratoId, decimal valor, DateTime vencimento)
        {
            if (contratoId == 0 || valor <= 0)
            {
                TempData["MensagemErro"] = "Preencha todos os campos corretamente.";
                return RedirectToAction("Administrador");
            }

            var novo = new Boleto
            {
                ContratoId = contratoId,
                Valor = valor,
                Vencimento = vencimento,
                Status = "Pendente"
            };

            _context.Boletos.Add(novo);
            await _context.SaveChangesAsync();

            try
            {
                // Gera o QR Code automaticamente
                return await GerarQrCode(novo.Id);
            }
            catch (Exception)
            {
                // Caso aconteça erro na geração, ainda retorna à tela principal
                TempData["MensagemErro"] = "Boleto criado, mas houve erro ao gerar o QR Code.";
                return RedirectToAction("Administrador");
            }
        }
    }
}