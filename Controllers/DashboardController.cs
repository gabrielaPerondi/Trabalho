using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrabalhoElvis2.Context;
using TrabalhoElvis2.Models;

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
            ViewBag.TotalCondominos = _context.Condominos.Count();
            ViewBag.TotalImoveis = _context.Imoveis.Count();
            ViewBag.TotalFuncionarios = _context.Funcionarios.Count();

            return View();
        }
    }
}