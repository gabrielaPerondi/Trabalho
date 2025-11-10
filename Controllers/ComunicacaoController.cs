using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabalhoElvis2.Context;
using TrabalhoElvis2.Models;

namespace Trabalho.Controllers
{
    public class ComunicacaoController : Controller
    {
        // GET: /Comunicacao/Index
        public IActionResult Index()
        {
            // apenas abre a página, sem alterar nada no menu
            return View();
        }
    }
}