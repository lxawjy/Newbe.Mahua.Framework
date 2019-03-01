using System.Threading.Tasks;

namespace Newbe.Mahua.InputReceivers.WebsocketApi
{
    public interface IMahuaWebsocketClientManager
    {
        Task StartNew(IMahuaWebsocketClient websocketClient);

        Task SendOutput(IOutput output);
    }
}