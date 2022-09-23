using AspNetCoreHero.ToastNotification.Abstractions;
using BuscaCnpj.Business.Interfaces;
using BuscaCnpj.Business.Services;
using BuscaCnpj.Business.Utils;
using BuscaCnpj.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BuscaCnpj.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConta _contaServices;
        private static readonly List<CnpjDiasVw> _listaLotes = new();
        private readonly INotyfService _toastNotification;

        public HomeController(IConta contaServices, INotyfService toastNotification)
        {
            _contaServices = contaServices;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public IActionResult Consultas()
        {
            _listaLotes.Clear(); //zera lista static POG
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Consultas(Root model, string cnpj)
        {
            try
            {
                if (!ModelState.IsValid) return View();
                if (cnpj == null) return View();
                cnpj = Utilidades.FormataCnpj(cnpj);

                if (cnpj.Length >= 14 && cnpj.Length <= 14) ViewBag.Lista = await _contaServices.BuscaContaPorCnpj(cnpj);

                _toastNotification.Success("CNPJ encontrado!");

                return View();
            }
            catch (Exception)
            {
                _toastNotification.Warning("CNPJ inexistente!");
                return View();
            }
        }

        [HttpGet]
        public IActionResult ConsultaDefasagem()
        {
            _listaLotes.Clear(); //zera lista static POG
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConsultaDefasagem(Root model, string cnpj, int dias)
        {
            try
            {
                if (!ModelState.IsValid) return View();

                ViewBag.Lista = null;
                if (cnpj != null) cnpj = Utilidades.FormataCnpj(cnpj);

                if (cnpj.Length >= 14 && cnpj.Length <= 14) ViewBag.Lista = await _contaServices.BuscaContaPorCnpjDefasado(cnpj, dias);
                _toastNotification.Success("CNPJ encontrado!");

                return View();
            }
            catch (Exception)
            {
                _toastNotification.Warning("CNPJ inexistente!");
                return View();
            }
        }

        [HttpGet]
        public IActionResult ConsultaLote(int? tipo)
        {
            try
            {
                if(tipo == 1) _toastNotification.Success("Arquivo gerado!");
                ViewBag.Lista = _listaLotes.Count >= 1 ? _listaLotes : null;
            }
            catch (Exception)
            {
                _toastNotification.Warning($"A API parou de responder!");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ConsultaLote(CnpjDiasVw model)
        {
            if (Utilidades.ValidaCnpj(model.Cnpj) is true)
            {
                _listaLotes.Add(model);
                ViewBag.Lista = _listaLotes.Distinct();
                ViewBag.Quantidade = _listaLotes.Distinct().Count();
            }
            return View();
        }

        [HttpGet]
        public IActionResult DeletaLote()
        {
            _listaLotes.Clear(); //zera lista static POG
            return RedirectToAction("ConsultaLote", new { lote = false });
        }

        public async Task<ActionResult> GeraExcel(string cnpj)
        {
            if (cnpj == null) return View();
            cnpj = Utilidades.FormataCnpj(cnpj);

            dynamic resultado = await ContaServices.EndPointCnpj(cnpj);
            StringBuilder builder = _contaServices.ConstroiCSV(resultado);

            _toastNotification.Success("CNPJ encontrado!");

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"{resultado.cnpj}.csv");
        }
        [HttpGet]
        public async Task<ActionResult> GeraLoteExcel()
        {
            var builder = new StringBuilder();
            try
            {
                builder = await _contaServices.ConstroiLoteCSV(_listaLotes.Distinct());
                ViewBag.Lista = null;
                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"loteCNPJ{DateTime.Now}.csv");
            }
            catch (Exception)
            {
                _toastNotification.Warning($"A API parou de responder!");
                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"loteCNPJ{DateTime.Now}.csv");
            }
        }
    }
}