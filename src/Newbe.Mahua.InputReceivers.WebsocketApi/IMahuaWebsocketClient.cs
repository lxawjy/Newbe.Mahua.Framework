using System.Threading.Tasks;

namespace Newbe.Mahua.InputReceivers.WebsocketApi
{
    public interface IMahuaWebsocketClient
    {
        Task Start();
        Task SendOutput(IOutput output);
    }
}