namespace Async_Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private int _counter = 0;
        private Task<string> _singleTask;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            
            Task<string> t1 = SingleTask("Greeting Earthlings!", 1000);

            _singleTask = SingleTask("\nWe come in peace.",1500);

            t1.Wait();

            Console.WriteLine(t1.Result);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool received = false;

            while (!stoppingToken.IsCancellationRequested)
            {
                if(_singleTask.IsCompleted && !received)
                {
                    Console.Write(_singleTask.Result);
                    received = true;
                }

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _counter++;
                Console.Write("\n_counter={0};", _counter);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<string> SingleTask(string str, int d)
        {
            Console.Write("\nIncoming message from the Boötes Void");

            for (int i = 0; i < 10; i++)
            {
                Console.Write(".");
                await Task.Delay(d);
            }

            return str;
        }
    }
}