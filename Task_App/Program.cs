using System.ComponentModel.Design;
using System.Dynamic;

namespace Task_App
{
    internal class Program
    {
        static Random _rand = new Random();
        static bool _uselock = false;
        static Semaphore _lock;
        static int _limit = 1;


        static void Main(string[] args)
        {
            Console.Write("Use lock? (y/n): ");

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (keyInfo.Key != ConsoleKey.Y && keyInfo.Key != ConsoleKey.N)
            {
                Console.SetCursorPosition(17, 0);
                Console.Write(" ");
                Console.SetCursorPosition(17, 0);

                keyInfo = Console.ReadKey();
            }
            Console.WriteLine();

            if (keyInfo.Key == ConsoleKey.Y)
            {
                _uselock = true;
                Console.Write("Enter max concurrent threads: ");
                string lim = Console.ReadLine();

                while ( !int.TryParse(lim, out _limit) && _limit < 1) // Ensure input is an positive integer.
                {
                    Console.SetCursorPosition(20, 1);
                    Console.Write("               ");
                    Console.SetCursorPosition(20, 1);

                    lim = Console.ReadLine();
                }

                Console.WriteLine(_limit);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Number of processors: {0} ", Environment.ProcessorCount);
            Console.ForegroundColor = ConsoleColor.White;

            _lock = new Semaphore(_limit, _limit, "OurLock"); // This is a named semaphore that can be used multiple processes running on the device.
            ///<remarks>
            /// SemaphoreSlim is more "lightweight" but can only be used to synchonize threads and to limit concurrency within a single process.
            /// The semaphore used above starts with the same number of open slots as max number of connurrent threads.
            ///</remarks>

            DateTime s = DateTime.Now;
            Task task1 = Say("\nFinally out of the great void in the universe\n.", 10000, "Great Boötes Void"); /// This task will complete after the next.
            Task task2 = Say("\nA first message from Earth.\n", 5000, "Earth"); /// This task will complete first.

            while (!task1.IsCompleted ) // Wait for a specific task to complete before exiting loop.
            {
                Console.Write(".");
                Task.Delay(250).Wait(); //Wait 250ms.
            }

            Console.WriteLine("\nExecution time: {0}", (DateTime.Now - s));
            Task.Delay(10000).Wait(); // Delay to allow remaining task to write to console before the app is terminated and kills the thread.

            Task.WaitAll();
        }

        static async Task Say(string str, int d, string id)
        {
            if (_uselock)
            {
                _lock.WaitOne();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n{0} is inside lock.\n", id);
                Console.ForegroundColor = ConsoleColor.White;

                await Task.Delay(d); //  Note that the await keyword can only be used in async methods.
                Console.Write(str);

                _lock.Release();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n{0} exited lock.\n", id);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                await Task.Delay(d); // Await keyword can only be used in async methods.
                Console.Write(str);
            }

            return;
        }

    }
}