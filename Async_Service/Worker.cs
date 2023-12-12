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
            Announce(t1.Id);
            
            _singleTask = SingleTask("We come in peace.",1500);
            Announce( _singleTask.Id );

            t1.Wait();


            DisplayMessage(t1.Id, t1.Result);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool received = false;

            while (!stoppingToken.IsCancellationRequested)
            {
                if(_singleTask.IsCompleted && !received)
                {
                    DisplayMessage(_singleTask.Id, _singleTask.Result);
                    received = true;
                }

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _counter++;
                Console.Write("\n_counter={0};", _counter);
                await Task.Delay(1000, stoppingToken);

                string str = await SingleTask("this is my string", 100);

            }
        }

        private async Task<string> SingleTask(string str, int d)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.Write(".");
                await Task.Delay(d);
            }

            return str;
        }

        private void DisplayMessage(int id, string message)
        {
            Console.Write("\n\nMessage {0}: {1}\n", id, message);
        }

        private void Announce(int id)
        {
            Console.Write("\nAwaiting message {0} from Boötes Void", id);
        }

    }
}