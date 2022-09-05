using BuscaCnpj.Models;
using System.Threading.Tasks;

namespace BuscaCnpj.Business.Interfaces
{
    public interface IConta
    {
        Task<Root> BuscaContaPorCnpj(string cnpj);
        Root PopulaInfoRootPorObj(dynamic obj);
    }
}