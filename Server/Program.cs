using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable 4014

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var listener = WebSocketListener.ListenOn("http://localhost:8764/");

            while (true)
            {
                var webSocket = await listener.WaitForNewConnectionAsync();
                Task.Run(async () =>
                {
                    var processor = new WebSocketConnectionProcessor(webSocket, cancellationTokenSource.Token);
                    await processor.ProcessAsync(
                        (m, r) =>
                        {
                            Console.WriteLine($"Received bytes: {r.Count}");
                            Console.WriteLine($"Received '{Encoding.ASCII.GetString(m)}'.");
                        });
                    Console.WriteLine("Connection closed");
                });
            }
        }
    }
}