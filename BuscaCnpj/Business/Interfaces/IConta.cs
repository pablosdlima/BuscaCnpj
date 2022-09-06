using BuscaCnpj.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BuscaCnpj.Business.Interfaces
{
    public interface IConta
    {
        Task<Root> BuscaContaPorCnpj(string cnpj);
        void CriaExcelPorObj(string conta);
        Root PopulaInfoRootPorObj(dynamic obj);
    }
}