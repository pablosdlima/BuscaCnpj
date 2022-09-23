using BuscaCnpj.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BuscaCnpj.Business.Interfaces
{
    public interface IConta
    {
        Task<Root> BuscaContaPorCnpj(string cnpj);
        Task<Root> BuscaContaPorCnpjDefasado(string cnpj, int dias);
        StringBuilder ConstroiCSV(dynamic resultado);
        Task<StringBuilder> ConstroiLoteCSV(IEnumerable<CnpjDiasVw> listaLotes);
        Root PopulaInfoRootPorObj(dynamic obj);
    }
}