using System.Net;
using System.Net.Sockets;

namespace Async_Service_WithPerformance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();

            Console.WriteLine("Starting worker...");

            host.Run();
        }
    }
}