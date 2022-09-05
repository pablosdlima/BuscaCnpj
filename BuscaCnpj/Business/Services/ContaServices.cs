using BuscaCnpj.Business.Interfaces;
using BuscaCnpj.Data;
using BuscaCnpj.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BuscaCnpj.Business.Services
{
    public class ContaServices : IConta
    {
        public async Task<Root> BuscaContaPorCnpj(string cnpj)
        {
            try
            {
                Root model = new();
                HttpClient http = new();
                http.BaseAddress = new Uri($"https://receitaws.com.br/v1/cnpj/");
                HttpResponseMessage resposta = await http.GetAsync(cnpj);
                resposta.EnsureSuccessStatusCode();

                string conteudo = resposta.Content.ReadAsStringAsync().Result;
                dynamic resultado = JsonConvert.DeserializeObject(conteudo);

                Root root = PopulaInfoRootPorObj(resultado);
                return root;
            }
            catch (Exception err)
            {
                Console.Write($"{err}");
            }
            return null;
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