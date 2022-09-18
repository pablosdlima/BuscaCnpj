using BuscaCnpj.Business.Interfaces;
using BuscaCnpj.Business.Services;
using BuscaCnpj.Business.Utils;
using BuscaCnpj.Models;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private static readonly List<dynamic> _listaLotes = new();
        public Boolean IsLoading { get; set; } = true;

        public HomeController(IConta contaServices)
        {
            _contaServices = contaServices;
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
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult ConsultaDefasagem()
        {
            _listaLotes.Clear(); //zera lista static POG
            return View();
        }

        [HttpGet]
        public IActionResult ConsultaLote(bool lote)
        {
            try
            {
                if (lote is true)
                {
                    List<dynamic> list = new();
                    var listaLotes = _listaLotes.Distinct();

                    var builder = new StringBuilder();
                    builder.AppendLine($"CNPJ ; STATUS ; ultima_atualizacao; Tipo;  Porte ;" +
                        " Nome ; Fantasia ; Abertura ;  Codigo atividade_principal ;  Texto atividade_principal;" +
                        " Codigo atividades_secundarias;  Texto atividades_secundarias;  natureza_juridica;" +
                        " logradouro; Numero;  Complemento;  Cep; Bairro;  Municipio;  UF; Email;" +
                        " Telefone;  EFR;  data_situacao;  motivo_situacao; situacao_especial;" +
                        " data_situacao_especial;  capital_social;");

                    foreach (var item in listaLotes)
                    {

                        builder.AppendLine($"{item.cnpj} ; {item.status} ; {item.ultima_atualizacao}; {item.tipo} ; {item.porte} ;" +
                            $"{item.nome}; {item.fantasia} ; {item.abertura} ; {item.atividade_principal[0].code};" +
                            $"{item.atividade_principal[0].text}; {item.atividades_secundarias[0].code}; {item.atividades_secundarias[0].text};" +
                            $"{item.natureza_juridica}; {item.logradouro}; {item.numero}; {item.complemento}; {item.cep};" +
                            $"{item.bairro}; {item.municipio}; {item.uf}; {item.email}; {item.telefone}; {item.efr};" +
                            $"{item.data_situacao}; {item.motivo_situacao}; {item.situacao_especial}; {item.data_situacao_especial}; {item.capital_social};");

                    }
                    ViewBag.Lista = null;
                    return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"loteCNPJ.csv");
                }
                else
                {
                    ViewBag.Lista = _listaLotes.Count >= 1 ? _listaLotes : null;
                }
                return View();
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConsultaLote(Root model, string cnpj)
        {
            if (!ModelState.IsValid) return View();
            if (_listaLotes.Count >= 3) Thread.Sleep(8000);
            
            var conta = await ContaServices.EndPointCnpj(Utilidades.FormataCnpj(cnpj));
            if (conta != null)
            {
                _listaLotes.Add(conta);
            }
            return RedirectToAction("ConsultaLote");
        }

        [HttpGet]
        public IActionResult DeletaLote()
        {
            _listaLotes.Clear(); //zera lista static POG
            return RedirectToAction("ConsultaLote", new { lote = false });
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
                return View();
            }
            catch (Exception)
            {
                return View();
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