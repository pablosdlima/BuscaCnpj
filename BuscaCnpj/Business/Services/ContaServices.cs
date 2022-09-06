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
        public async Task<Root> BuscaContaPorCnpj(string cnpj)
        {
            try
            {
                //Root model = new();
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

        public async void CriaExcelPorObj(string cnpj)
        {
            try
            {
                HttpClient http = new();
                http.BaseAddress = new Uri($"https://receitaws.com.br/v1/cnpj/");
                HttpResponseMessage resposta = await http.GetAsync(cnpj);
                resposta.EnsureSuccessStatusCode();
                string conteudo = resposta.Content.ReadAsStringAsync().Result;
                dynamic resultado = JsonConvert.DeserializeObject(conteudo);


                //DataTable dt = new DataTable();
                //dt.Columns.Add("Nome");
                //dt.Columns.Add("Telefone");
                //dt.Rows.Add("eduardo", "11111");
                //dt.Rows.Add("coutinho", "22222");

                //FileInfo fileInfoTemplate = new FileInfo(System.Web.HttpContext.Current.Server.MapPath("~/Template/Template.xlsx"));

                //OfficeOpenXml.ExcelPackage excel = new ExcelPackage(fileInfoTemplate);

                //ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("teste");
                //worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                //System.IO.MemoryStream stream = new System.IO.MemoryStream();
                //excel.SaveAs(stream);


                //HttpContext.Response.Clear();
                //HttpContext.Response.AddHeader("content-disposition", string.Format("attachment;filename=Teste_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));

                //HttpContext.Response.ContentType = "application/vnd.ms-excel";
                //HttpContext.Response.ContentEncoding = System.Text.Encoding.Default;

                //HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                //stream.WriteTo(Response.OutputStream);

                //Response.End();

            }
            catch (Exception)
            {
                throw;
            }
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