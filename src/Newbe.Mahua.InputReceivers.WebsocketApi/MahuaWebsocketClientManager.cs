using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Newbe.Mahua.InputReceivers.WebsocketApi
{
    /// <summary>
    /// manage websocket client.
    /// NOTICE: using static class and fields , because it have to share instance between web ioc container and mahua ioc container.
    /// You can find register as singleton in Startup.cs and MahuaModule.cs , but they are different instance, because they are in different container. so using static fields to share instance.
    /// </summary>
    internal class MahuaWebsocketClientManager : IMahuaWebsocketClientManager
    {
        private static readonly List<IMahuaWebsocketClient> Clients = new List<IMahuaWebsocketClient>();

        private static Task StartNew(IMahuaWebsocketClient websocketClient)
        {
            Clients.Add(websocketClient);
            return websocketClient.Start();
        }

        private static Task SendOutput(IOutput output)
        {
            return Task.WhenAll(Clients.Select(x => x.SendOutput(output)));
        }

        Task IMahuaWebsocketClientManager.SendOutput(IOutput output)
        {
            return SendOutput(output);
        }

        Task IMahuaWebsocketClientManager.StartNew(IMahuaWebsocketClient websocketClient)
        {
            return StartNew(websocketClient);
        }
    }
}