using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Server
{
    internal sealed class WebSocketConnectionProcessor : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ArraySegment<byte> _receiveBuffer = new(new byte[1024]);
        private readonly Thread _thread;
        private readonly WebSocket _webSocket;

        public WebSocketConnectionProcessor(WebSocket webSocket)
        {
            _webSocket = webSocket;
            _thread = new Thread(Worker);
            _thread.Start();
        }

        private void Worker()
        {
            Console.WriteLine("Connection opened");
            
            while (_webSocket.State == WebSocketState.Open && !_cancellationTokenSource.IsCancellationRequested)
            {
                _webSocket.ReceiveAsync(_receiveBuffer, _cancellationTokenSource.Token).Wait();
                Console.WriteLine(Encoding.ASCII.GetString(_receiveBuffer));
                _webSocket.SendAsync(
                    _receiveBuffer,
                    WebSocketMessageType.Text,
                    true,
                    _cancellationTokenSource.Token).Wait();
            }
            
            Console.WriteLine("Connection closed");
        }

        public void Dispose()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                _thread.Join();
                _webSocket.Dispose();
            }
        }
    }
}