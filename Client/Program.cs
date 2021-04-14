using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8764/");
            httpListener.Start();

            HttpListenerContext context = await httpListener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                WebSocket webSocket = webSocketContext.WebSocket;
                var receiveBuffer = new ArraySegment<byte>(new byte[1024]);
                webSocket.ReceiveAsync(receiveBuffer, CancellationToken.None)
                    .ContinueWith(t =>
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(receiveBuffer));
                    });
                
                while (webSocket.State == WebSocketState.Open)
                {
                    Thread.Sleep(1000);
                    webSocket.SendAsync(
                        new ArraySegment<byte>(Encoding.ASCII.GetBytes("Ovo su neki podaci")),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None).Wait();
                }
            }
        }
    }
}