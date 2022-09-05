using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BuscaCnpj.Business.Hubs
{
    public class SignalServer: Hub
    {
        public async Task BuscaCnpj()
        {
            await Clients.All.SendAsync("SignalConsultaCnpj", null);
        }
    }
}
