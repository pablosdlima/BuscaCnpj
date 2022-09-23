using BuscaCnpj.Business.Interfaces;
using BuscaCnpj.Business.Private;
using BuscaCnpj.Business.Utils;
using BuscaCnpj.Data;
using BuscaCnpj.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BuscaCnpj.Business.Services
{
    public class ContaServices : IConta
    {
        private static HttpClient HttpAuthAPI()
        {

            HttpClient http = new(); //API
            http.BaseAddress = new Uri($"https://receitaws.com.br/v1/");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", $"{Secret.Key}"
            );
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
            HttpClient http = HttpAuthAPI();
            HttpResponseMessage resposta = await http.GetAsync($"cnpj/{cnpj}");
            return ResponseStatusCode(resposta);
        }

        public static async Task<dynamic> EndPointCnpjDefasado(string cnpj, int days)
        {
            HttpClient http = HttpAuthAPI();
            HttpResponseMessage resposta = await http.GetAsync($"cnpj/{cnpj}/days/{days}");
            return ResponseStatusCode(resposta);
        }

        public static async Task<dynamic> EndPointRelatorioCnpj()
        {
            HttpClient http = HttpAuthAPI();
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

        public async Task<StringBuilder> ConstroiLoteCSV(IEnumerable<CnpjDiasVw> listaLotes)
        {
            try
            {
                int contador = 0;
                var builder = new StringBuilder();
                builder.AppendLine($"CNPJ ; STATUS ; ultima_atualizacao; Tipo;  Porte ;" +
                    " Nome ; Fantasia ; Abertura ;  Codigo atividade_principal ;  Texto atividade_principal;" +
                    " Codigo atividades_secundarias;  Texto atividades_secundarias;  natureza_juridica;" +
                    " logradouro; Numero;  Complemento;  Cep; Bairro;  Municipio;  UF; Email;" +
                    " Telefone;  EFR;  data_situacao;  motivo_situacao; situacao_especial;" +
                    " data_situacao_especial;  capital_social;");

                foreach (var item in listaLotes)
                {
                    contador++;
                    if (contador == 3 || contador == 6 || contador == 9 || contador == 12 || contador == 15) Thread.Sleep(1000);

                    var conta = await ContaServices.EndPointCnpjDefasado(Utilidades.FormataCnpj(item.Cnpj), item.Dias);
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
                return builder;
            }
            catch (Exception err)
            {
                Console.WriteLine("teste");
                throw;
            }
        }

        public static string RetiraAcentos(string texto)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
            }
            texto = Regex.Replace(texto, "[^0-9a-zA-Z]+", "");
            return texto;
        }

    }
}