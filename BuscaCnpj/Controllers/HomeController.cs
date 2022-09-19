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
        //private static readonly List<string> _listaLotes = new();
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
        public async Task<IActionResult> ConsultaLote(bool lote)
        {
            try
            {
                if (lote is true)
                {
                    //List<dynamic> list = new();
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
                        Thread.Sleep(1000);
                        var conta = await ContaServices.EndPointCnpjDefasado(Utilidades.FormataCnpj(item), 1);
                        if (conta != null)
                        {
                            builder.AppendLine($"{conta.cnpj} ; {conta.status} ; {conta.ultima_atualizacao}; {conta.tipo} ; {conta.porte} ;" +
                                $"{conta.nome}; {conta.fantasia} ; {conta.abertura} ; {conta.atividade_principal[0].code};" +
                                $"{conta.atividade_principal[0].text}; {conta.atividades_secundarias[0].code}; {conta.atividades_secundarias[0].text};" +
                                $"{conta.natureza_juridica}; {conta.logradouro}; {conta.numero}; {conta.complemento}; {conta.cep};" +
                                $"{conta.bairro}; {conta.municipio}; {conta.uf}; {conta.email}; {conta.telefone}; {conta.efr};" +
                                $"{conta.data_situacao}; {conta.motivo_situacao}; {conta.situacao_especial}; {conta.data_situacao_especial}; {conta.capital_social};");
                        }
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
        public IActionResult ConsultaLote(Root model, string cnpj)
        {
            _listaLotes.Add(Utilidades.FormataCnpj(cnpj));
            ViewBag.Lista = _listaLotes;
            return View();
        }

        // [HttpPost]
        //public async Task<IActionResult> ConsultaLote(Root model, string cnpj)
        //{
        //    if (!ModelState.IsValid) return View();
        //    var conta = await ContaServices.EndPointCnpjDefasado(Utilidades.FormataCnpj(cnpj), 1);
        //    if (conta != null)
        //    {
        //        _listaLotes.Add(conta);
        //    }
        //    return RedirectToAction("ConsultaLote");
        //}

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