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

        public async Task<ActionResult> GeraExcel(string cnpj)
        {
            if (cnpj == null) return View();
            cnpj = Utilidades.FormataCnpj(cnpj);

            dynamic resultado = await ContaServices.ConsomeEndPointCnpj(cnpj);

            var builder = new StringBuilder();
            builder.AppendLine($"CNPJ ; STATUS ; ultima_atualizacao;");
            builder.AppendLine($"{resultado.cnpj} ; {resultado.status} ; {resultado.ultima_atualizacao}");
            builder.AppendLine($"");
            builder.AppendLine($"Tipo ; Porte ; Nome ;");
            builder.AppendLine($"{resultado.tipo} ; {resultado.porte} ; {resultado.nome}");
            builder.AppendLine($"");
            builder.AppendLine($"Fantasia ; Abertura ; Codigo atividade_principal ; Texto atividade_principal; ");
            builder.AppendLine($"{resultado.fantasia} ; {resultado.abertura} ; {resultado.atividade_principal[0].code}; {resultado.atividade_principal[0].text} ");
            builder.AppendLine($"");
            builder.AppendLine($"Codigo atividades_secundarias; Texto atividades_secundarias; natureza_juridica; logradouro");
            builder.AppendLine($"{resultado.atividades_secundarias[0].code};{resultado.atividades_secundarias[0].text}; {resultado.natureza_juridica}; {resultado.logradouro}");
            builder.AppendLine($"");
            builder.AppendLine($"Numero; Complemento; Cep;");
            builder.AppendLine($"{resultado.numero}; {resultado.complemento}; {resultado.cep}");
            builder.AppendLine($"");
            builder.AppendLine($"Bairro; Municipio; UF;");
            builder.AppendLine($"{resultado.bairro}; {resultado.municipio}; {resultado.uf};");
            builder.AppendLine($"");
            builder.AppendLine($"Email; Telefone; EFR");
            builder.AppendLine($"{resultado.email}; {resultado.telefone}; {resultado.efr}");
            builder.AppendLine($"");
            builder.AppendLine($"Situação; data_situacao; motivo_situacao");
            builder.AppendLine($"{resultado.situacao}; {resultado.data_situacao}; {resultado.motivo_situacao}");
            builder.AppendLine($"");
            builder.AppendLine($"situacao_especial; data_situacao_especial; capital_social");
            builder.AppendLine($"{resultado.situacao_especial}; {resultado.data_situacao_especial}; {resultado.capital_social}");
            //builder.AppendLine($"qsa; free billing; database billing; ;");
            //builder.AppendLine($"{resultado.qsa}; {resultado.billing[0].free.toString()}; {resultado.billing[0].database.toString()}; ;");

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "cnpj.csv");
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