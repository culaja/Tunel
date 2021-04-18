using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var listener = WebSocketListener.ListenOn("http://localhost:8764/");
            var connectionQueue = new Queue<WebSocketConnectionProcessor>();

            while (true)
            {
                var webSocket = await listener.WaitForNewConnectionAsync();
                connectionQueue.Enqueue(new WebSocketConnectionProcessor(webSocket));
            }
        }
    }
}