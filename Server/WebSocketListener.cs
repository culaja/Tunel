using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Server
{
    internal sealed class WebSocketListener : IDisposable
    {
        private readonly HttpListener _httpListener;

        private WebSocketListener(HttpListener httpListener)
        {
            _httpListener = httpListener;
        }

        public static WebSocketListener ListenOn(string listenUrl)
        {
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add(listenUrl);
            httpListener.Start();
            return new WebSocketListener(httpListener);
        }

        public async Task<WebSocket> WaitForNewConnectionAsync()
        {
            WebSocket? webSocket = null;
            do
            {
                var context = await _httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var webSocketContext = await context.AcceptWebSocketAsync(null!);
                    webSocket = webSocketContext.WebSocket;
                }
            } while (webSocket == null);

            return webSocket;
        }

        public void Dispose()
        {
            if (_httpListener.IsListening)
            {
                _httpListener.Stop();
            }
        }
    }
}