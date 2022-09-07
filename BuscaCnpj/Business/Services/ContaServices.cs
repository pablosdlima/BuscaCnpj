using BuscaCnpj.Business.Interfaces;
using BuscaCnpj.Data;
using BuscaCnpj.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BuscaCnpj.Business.Services
{
    public class ContaServices : IConta
    {
        public static async Task<dynamic> ConsomeEndPointCnpj(string cnpj)
        {
            HttpClient http = new();
            http.BaseAddress = new Uri($"https://receitaws.com.br/v1/cnpj/");
            HttpResponseMessage resposta = await http.GetAsync(cnpj);
            resposta.EnsureSuccessStatusCode();

            string conteudo = resposta.Content.ReadAsStringAsync().Result;
            dynamic resultado = JsonConvert.DeserializeObject(conteudo);
            return resultado;
        }

        public async Task<Root> BuscaContaPorCnpj(string cnpj)
        {
            try
            {

                dynamic resultado = await ConsomeEndPointCnpj(cnpj);
                Root root = PopulaInfoRootPorObj(resultado);
                return root;
            }
            catch (Exception err)
            {
                Console.Write($"{err}");
            }
            return null;
        }

        //public async FileResult CriaExcelPorObj(string cnpj)
        //{
        //    try
        //    {
        //        dynamic resultado = await ConsomeEndPointCnpj(cnpj);

        //        var builder = new StringBuilder();
        //        builder.AppendLine("Cnpj, Cep");
        //        builder.AppendLine($"{resultado.cnpj}, {resultado.cep}");

        //        return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "users.csv");


        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public Root PopulaInfoRootPorObj(dynamic obj)
        {
            Root root = new()
            {
                cnpj = obj.cnpj,
                nome = obj.nome,
                fantasia = obj.fantasia,
                cep = obj.cep
            };
            return root;
        }
    }
}