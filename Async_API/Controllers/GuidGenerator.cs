using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Async_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GuidGeneratorController : ControllerBase
    {
        private readonly ILogger<GuidGeneratorController> _logger;

        public GuidGeneratorController(ILogger<GuidGeneratorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<Guid[]> Get(int size = 100)
        {
            Console.WriteLine("API called.");

            Task t1 = Say("Greetings from the Boötes Void.");
            Task<Guid[]> t2 = FillArray(size);

            Console.WriteLine("Generating globally unique identifiers.");
            await Task.Delay(5000);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Returning result.");
            Console.ForegroundColor = ConsoleColor.White;

            t2.Wait();

            return t2.Result;
        }

        private async Task Say(string str)
        {
            await Task.Delay(10000);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private async Task<Guid[]> FillArray(int size)
        {
            Guid[] ids = new Guid[size];

            for(int i = 0; i < size; i++)
            {
                ids[i] = Guid.NewGuid();
            }

            return ids;
        }
    }
}