using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IO;
using Newtonsoft.Json;

namespace Newbe.Mahua.InputReceivers.WebsocketApi
{
    internal class MahuaWebsocketClient : IMahuaWebsocketClient
    {
        public delegate MahuaWebsocketClient Factory(WebSocket webSocket);

        private readonly IMahuaCenter _mahuaCenter;
        private readonly WebSocket _webSocket;

        public MahuaWebsocketClient(
            WebSocket webSocket,
            IMahuaCenter mahuaCenter)
        {
            _mahuaCenter = mahuaCenter;
            _webSocket = webSocket;
        }

        public async Task Start()
        {
            var buffer = new byte[1024 * 4];
            var recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                var recyclableMemoryStream = new RecyclableMemoryStream(recyclableMemoryStreamManager);
                do
                {
                    await recyclableMemoryStream.WriteAsync(buffer, 0, result.Count);
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                } while (!result.EndOfMessage);

                var inputString = Encoding.UTF8.GetString(recyclableMemoryStream.GetBuffer());
                var input = JsonConvert.DeserializeObject<WebsocketInput>(inputString);
                await _mahuaCenter.HandleMahuaInput(input);
            }

            await _webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,
                CancellationToken.None);
        }

        public Task SendOutput(IOutput output)
        {
            var json = JsonConvert.SerializeObject(output);
            var bytes = Encoding.UTF8.GetBytes(json);
            if (_webSocket.State == WebSocketState.Open)
            {
                return _webSocket.SendAsync(new ArraySegment<byte>(bytes), 0, true, CancellationToken.None);
            }

            return Task.CompletedTask;
        }
    }
}