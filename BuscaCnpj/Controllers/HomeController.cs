using BuscaCnpj.Business.Interfaces;
using BuscaCnpj.Business.Utils;
using BuscaCnpj.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BuscaCnpj.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConta _contaServices;

        public HomeController(IConta contaServices)
        {
            _contaServices = contaServices;
        }

        [HttpGet]
        public IActionResult Consultas()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Consultas(string cnpj)
        {
            try
            {
                if (cnpj == null) return View();
                cnpj = Utilidades.FormataCnpj(cnpj);

                if(cnpj.Length >= 14 && cnpj.Length <= 14) ViewBag.Lista = await _contaServices.BuscaContaPorCnpj(cnpj);

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

       

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}