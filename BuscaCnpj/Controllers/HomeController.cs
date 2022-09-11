using BuscaCnpj.Business.Interfaces;
using BuscaCnpj.Business.Services;
using BuscaCnpj.Business.Utils;
using BuscaCnpj.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        [HttpGet]
        public IActionResult ConsultaDefasagem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConsultaDefasagem(string cnpj, int dias)
        {
            try
            {
                ViewBag.Lista = null;
                if (cnpj == null) return null;
                cnpj = Utilidades.FormataCnpj(cnpj);

                if(cnpj.Length >= 14 && cnpj.Length <= 14) ViewBag.Lista = await _contaServices.BuscaContaPorCnpjDefasado(cnpj, dias);
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //[HttpGet]
        //public IActionResult RelatorioConsultas()
        //{
        //    return View();
        //}

        //[HttpPost]
        public async Task<IActionResult> RelatorioConsultas()
        {
            try
            {
              
                ViewBag.Lista = await _contaServices.BuscaRelatorioCnpj();

                Console.WriteLine("Teste");
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ActionResult> GeraExcel(string cnpj)
        {
            if (cnpj == null) return View();
            cnpj = Utilidades.FormataCnpj(cnpj);

            dynamic resultado = await ContaServices.EndPointCnpj(cnpj);
            StringBuilder builder = _contaServices.ConstroiCSV(resultado);
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"{resultado.cnpj}.csv");
        }
    }
}