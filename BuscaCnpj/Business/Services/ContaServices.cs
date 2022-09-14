using BuscaCnpj.Business.Interfaces;
using BuscaCnpj.Business.Private;
using BuscaCnpj.Data;
using BuscaCnpj.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BuscaCnpj.Business.Services
{
    public class ContaServices : IConta
    {
        private static HttpClient HttpAuthAPI(bool apiKey)
        {

            HttpClient http = new(); //API
            http.BaseAddress = new Uri($"https://receitaws.com.br/v1/");

            if (apiKey == true)
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer", $"{Secret.Key}"
                );
                return http;
            }
            return http;
        }
        private static dynamic ResponseStatusCode(HttpResponseMessage resposta)
        {
            resposta.EnsureSuccessStatusCode();
            string conteudo = resposta.Content.ReadAsStringAsync().Result;
            dynamic resultado = JsonConvert.DeserializeObject(conteudo);
            return resultado;
        }

        public static async Task<dynamic> EndPointCnpj(string cnpj)
        {
            HttpClient http = HttpAuthAPI(false);
            HttpResponseMessage resposta = await http.GetAsync($"cnpj/{cnpj}");
            return ResponseStatusCode(resposta);
        }

        public static async Task<dynamic> EndPointCnpjDefasado(string cnpj, int days)
        {
            HttpClient http = HttpAuthAPI(true);
            HttpResponseMessage resposta = await http.GetAsync($"cnpj/{cnpj}/days/{days}");
            return ResponseStatusCode(resposta);
        }

        public static async Task<dynamic> EndPointRelatorioCnpj()
        {
            HttpClient http = HttpAuthAPI(true);
            HttpResponseMessage resposta = await http.GetAsync($"/account/calls/report");
            return ResponseStatusCode(resposta);
        }

        public async Task<Root> BuscaContaPorCnpj(string cnpj)
        {
            try
            {
                dynamic resultado = await EndPointCnpj(cnpj);
                Root root = PopulaInfoRootPorObj(resultado);
                return root;
            }
            catch (Exception err)
            {
                Console.Write($"{err}");
            }
            return null;
        }

        public async Task<Root> BuscaContaPorCnpjDefasado(string cnpj, int dias)
        {
            try
            {
                dynamic resultado = await EndPointCnpjDefasado(cnpj, dias);
                Root root = PopulaInfoRootPorObj(resultado);
                return root;
            }
            catch (Exception err)
            {
                Console.Write($"{err}");
            }
            return null;
        }


        //public async Task<dynamic> BuscaRelatorioCnpj()
        //{
        //    try
        //    {
        //        dynamic resultado = await EndPointRelatorioCnpj();
        //        resultado = resultado.calls;
        //        return resultado;
        //    }
        //    catch (Exception err)
        //    {
        //        Console.Write($"{err}");
        //    }
        //    return null;
        //}


        public StringBuilder ConstroiCSV(dynamic resultado)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"CNPJ ; STATUS ; ultima_atualizacao; Tipo;  Porte ;" +
                " Nome ; Fantasia ; Abertura ;  Codigo atividade_principal ;  Texto atividade_principal;" +
                " Codigo atividades_secundarias;  Texto atividades_secundarias;  natureza_juridica;" +
                " logradouro; Numero;  Complemento;  Cep; Bairro;  Municipio;  UF; Email;" +
                " Telefone;  EFR;  data_situacao;  motivo_situacao; situacao_especial;" +
                " data_situacao_especial;  capital_social;");

            builder.AppendLine($"{resultado.cnpj} ; {resultado.status} ; {resultado.ultima_atualizacao}; {resultado.tipo} ; {resultado.porte} ;" +
                $"{resultado.nome}; {resultado.fantasia} ; {resultado.abertura} ; {resultado.atividade_principal[0].code};" +
                $"{resultado.atividade_principal[0].text}; {resultado.atividades_secundarias[0].code}; {resultado.atividades_secundarias[0].text};" +
                $"{resultado.natureza_juridica}; {resultado.logradouro}; {resultado.numero}; {resultado.complemento}; {resultado.cep};" +
                $"{resultado.bairro}; {resultado.municipio}; {resultado.uf}; {resultado.email}; {resultado.telefone}; {resultado.efr};" +
                $"{resultado.data_situacao}; {resultado.motivo_situacao}; {resultado.situacao_especial}; {resultado.data_situacao_especial}; {resultado.capital_social};");
            return builder;
        }

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