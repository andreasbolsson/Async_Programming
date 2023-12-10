using System.Net;
using System.Net.Sockets;

namespace Async_Service_WithPerformance
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private readonly int _port;
        private TcpListener _listener;

        public Worker(IConfiguration config, ILogger<Worker> logger)
        {
            _config = config;
            _logger = logger;

            _port = config.GetValue<int>("Port"); // Get port from configuration file.

            IPAddress addr;

            if (IPAddress.TryParse("127.0.0.1", out addr)) // Listen to local TCP traffic on port set in configuration file.
            {
                _listener = new TcpListener(addr, _port);
            }

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(_listener != null)
            { 
            _listener.Start();

            Console.WriteLine("Listening on port {0}.\n", _port);

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_listener.Pending()) // Check if there is an incoming request.
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nINCOMING MESSAGE..................");
                        Console.WriteLine("----------------------------------");
                        Console.ForegroundColor = ConsoleColor.White;

                        using (TcpClient client = _listener.AcceptTcpClient()) // Create a client to read the incoming TCP stream/
                        {
                            NetworkStream stream = client.GetStream();
                            byte[] buffer = new byte[4096]; // Buffer to read chunks of data from the stream.

                            int n = stream.Read(buffer, 0, buffer.Length);

                            while (n > 0)
                            {
                                for (int i = 0; i < n; i++)
                                {
                                    if (buffer[i] > 0)
                                    {
                                        Char c = Convert.ToChar(buffer[i]);
                                        Console.Write(c);
                                    }
                                }

                                n = stream.Read(buffer, 0, buffer.Length);
                            }
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n---------END OF MESSAGE-----------");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    await Task.Delay(100, stoppingToken);
                }
            }
        }
    }
}