using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Async_Service_WithPerformance
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private readonly int _portStart;
        private TcpListener[] _listeners;
        private List<Task> _tasks;

        System.Diagnostics.PerformanceCounter _pcounter;

        public Worker(IConfiguration config, ILogger<Worker> logger)
        {
            _config = config;
            _logger = logger;
            _listeners = new TcpListener[3];
            _tasks = new List<Task>();

            if( !System.Diagnostics.PerformanceCounterCategory.Exists("BytesPerNS"))
            {
                System.Diagnostics.CounterCreationDataCollection dataCollection = new System.Diagnostics.CounterCreationDataCollection();

                System.Diagnostics.CounterCreationData counterData = new System.Diagnostics.CounterCreationData();
                counterData.CounterName = "TestService";
                counterData.CounterType = System.Diagnostics.PerformanceCounterType.NumberOfItems64;
                dataCollection.Add(counterData);

                System.Diagnostics.PerformanceCounterCategory.Create("BytesPerNS", "Counts number of bytes processed per nanosecond.", System.Diagnostics.PerformanceCounterCategoryType.SingleInstance, dataCollection);

                while (!System.Diagnostics.PerformanceCounterCategory.Exists("BytesPerNS"))
                {

                    Task.Delay(1000); // Wait for creation of performance counter to complete
                }
            }

            _pcounter = new System.Diagnostics.PerformanceCounter("BytesPerNS","TestService",false);

            _portStart = config.GetValue<int>("Port"); // Get port from configuration file.

            IPAddress addr;

            int p = _portStart;

            if (IPAddress.TryParse("127.0.0.1", out addr)) // Listen to local TCP traffic starting on port set in configuration file.
            {
                for (int k = 0; k < _listeners.Length; k++)
                {
                    _listeners[k] = new TcpListener(addr, p);
                    p++;
                }
            }
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            for (int k = 0; k < _listeners.Length; k++)
            {
                if (_listeners[k] != null)
                {
                    _tasks.Add(Listen(k, stoppingToken)); // Start another listener.
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Do nothing. Listening is handled by Listen(int index, CancellationToken stoppingToken)
        }

        private async Task Listen(int index, CancellationToken stoppingToken)
        {
            _listeners[index].Start();

            Console.WriteLine("Listening on port {0}.\n", ((IPEndPoint)_listeners[index].LocalEndpoint).Port);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_listeners[index].Pending()) // Check if there is an incoming request.
                {
                    TcpClient client = await _listeners[index].AcceptTcpClientAsync();

                    if (client != null)
                    {
                        //Task t = ProcessMessage(_listeners[k]); // Start independent thread to process incoming message.
                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nINCOMING MESSAGE ON PORT {0}..................", ((IPEndPoint)_listeners[index].LocalEndpoint).Port);
                            Console.ForegroundColor = ConsoleColor.White;

                            ReceiveMessage(client);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n--END OF MESSAGE--");
                            Console.ForegroundColor = ConsoleColor.White;

                        }
                        catch (Exception exc)
                        {
                            _logger.Log(LogLevel.Error, exc.Message,new object[0]); // Record exception.
                        }
                        finally
                        {
                            client.Close(); //Make sure connection is closed so listener continues listen to tcp traffic.

                        }
                        client.Close();
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;

                await Task.Delay(100, stoppingToken);
            }
        }
        private void ReceiveMessage( TcpClient client )
        {
            long x = 0;

            DateTime timeZero = DateTime.UtcNow;
            DateTime time = DateTime.UtcNow;
            TimeSpan dur = TimeSpan.Zero;

            long count = 0;

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096]; // Buffer to read chunks of data from the stream.

            int n = stream.Read(buffer, 0, buffer.Length);
            count = n;

            while (n > 0)
            {
                time = DateTime.UtcNow;

                OutputToConsole(buffer, n); //Message won't necessarily be in order, but make asunc so performance monitor measure speed of receiving bytes.

                dur = DateTime.UtcNow - time;
                _pcounter.RawValue = dur.Ticks * 100 / n;

                n = stream.Read(buffer, 0, buffer.Length);
                count += n;
            }

            
            dur = DateTime.UtcNow - timeZero;


            if (count > 0) // Prevent division by zero error just in case (should not happen though).
            {
                x = dur.Ticks * 100 / count;
            }


            Console.WriteLine("\nbytes/ns: {0}", x);
        }

        static async Task OutputToConsole( byte[] b, int r)
        {
            for (int i = 0; i < r; i++)
            {
                if (b[i] > 0)
                {
                    Char c = Convert.ToChar(b[i]);
                    Console.Write(c);
                }
            }
        }
    }
}