using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoElvis2.Context;

namespace TrabalhoElviss2.Controllers
{
    public class DashboardController : Controller
    {
        private readonly LoginContext _context;

        public DashboardController(LoginContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // TOTAL DE IMÓVEIS
            ViewBag.TotalImoveis = _context.Imoveis.Count();

            // OCUPAÇÃO
            ViewBag.QtdOcupados = _context.Imoveis.Count(i => i.Status == "Ocupado");
            ViewBag.QtdVagos = _context.Imoveis.Count(i => i.Status == "Vago");

            // CONTRATOS ATIVOS
            ViewBag.ContratosAtivos = _context.Contratos.Count(c => c.Status == "Ativo");

            // INADIMPLÊNCIA (Boletos vencidos ou pendentes)
            ViewBag.Inadimplencia = _context.Boletos
                .Where(b => b.Status == "Vencido" || b.Status == "Pendente")
                .Sum(b => (decimal?)b.Valor) ?? 0;

            // RECEITA MENSAL = boletos pagos no mês atual
            var hoje = DateTime.Now;
            ViewBag.ReceitaMensal = _context.Boletos
                .Where(b => b.Pagamento.HasValue &&
                            b.Pagamento.Value.Month == hoje.Month &&
                            b.Pagamento.Value.Year == hoje.Year)
                .Sum(b => (decimal?)b.Valor) ?? 0;

            // ATIVIDADES RECENTES (últimos contratos criados)
            ViewBag.Atividades = _context.Contratos
                .Include(c => c.Imovel)
                .OrderByDescending(c => c.DataInicio)
                .Take(5)
                .Select(c => new
                {
                    Mensagem = $"Contrato criado para o imóvel {c.Imovel.Codigo}",
                    Data = c.DataInicio.ToString("dd/MM/yyyy")
                })
                .ToList();

            if (ViewBag.Atividades == null)
                ViewBag.Atividades = new List<object>();

            return View();
        }
    }
}
