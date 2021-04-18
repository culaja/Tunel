using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal sealed class WebSocketConnectionProcessor : IDisposable
    {
        
        private readonly WebSocket _webSocket;
        private readonly CancellationToken _token;

        public WebSocketConnectionProcessor(WebSocket webSocket, CancellationToken token)
        {
            _webSocket = webSocket;
            _token = token;
        }

        public async Task ProcessReceivedAsync(
            Action<ArraySegment<byte>, WebSocketReceiveResult> onMessageReceived)
        {
            var  receiveBuffer = new ArraySegment<byte>(new byte[1024]);
            
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(receiveBuffer, _token);
                onMessageReceived(receiveBuffer, result);
            }
        }

        public Task Send(ArraySegment<byte> arraySegment) =>
            _webSocket.SendAsync(
                arraySegment,
                WebSocketMessageType.Text,
                true,
                _token);

        public void Dispose()
        {
            _webSocket.Dispose();
        }
    }
}