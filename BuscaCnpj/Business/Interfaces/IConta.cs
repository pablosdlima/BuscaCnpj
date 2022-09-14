using BuscaCnpj.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace BuscaCnpj.Business.Interfaces
{
    public interface IConta
    {
        Task<Root> BuscaContaPorCnpj(string cnpj);
        Task<Root> BuscaContaPorCnpjDefasado(string cnpj, int dias);
        //Task<dynamic> BuscaRelatorioCnpj();
        StringBuilder ConstroiCSV(dynamic resultado);
        Root PopulaInfoRootPorObj(dynamic obj);
    }
}