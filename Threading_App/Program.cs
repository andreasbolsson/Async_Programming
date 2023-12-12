using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

internal class Program
{
    /// 
    /// <summary>
    /// This sample application shows various ways for how to synchronyze work in a multithreaded application.
    /// </summary>
    /// <remarks>
    /// 1. Start app.
    /// 2. Press M key to switch mutual exlusion on and off.
    /// </remarks>
    private static Semaphore _lock; // A semaphore shared by all threads that limits the number of threads concurrently working on resouces. 
    private static Thread[] _threads;
    private static Random _rand = new Random();
    private static bool _exit = false;
    private static int _counter = 0;
    private static int _balance = 0;
    private static int _steps = 1;
    private static bool _useMutex = true;


    private static Mutex _mutex = new Mutex(); // Mutual exlusion synchonizer to limit work to a single thread.

    private static void Main(string[] args)
    {
        Console.WriteLine("Classic threading example with self-managed thread pool.");

        _exit = false;
        _lock = new Semaphore(0,5);
        _threads = new Thread[5];

    int i = 0;
        int len = _threads.Length;
        
        for(i=0; i<len; i++)
        {
            _threads[i] = new Thread(() => { DoWork(); });
            _threads[i].Name = i.ToString();    
            _threads[i].Start();
        }

        _lock.Release(5);
        Console.WriteLine("Threads started.");

        ConsoleKeyInfo kinfo = Console.ReadKey();
        Console.WriteLine("\r  ");

        while (kinfo.Key != ConsoleKey.X)
        {
            switch(kinfo.Key)
            {

                case ConsoleKey.M:
                    _useMutex = !_useMutex;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Switch mutual exclusion {0}.", _useMutex? "ON":"OFF" );
                    Console.ForegroundColor = ConsoleColor.White;

                    break;
                case ConsoleKey.N:
                    //Interlocked.Exchange(ref _useMutex, 0);
                    break;
                default:
                    break;
                //

            }

            kinfo = Console.ReadKey();
            Console.WriteLine("\r  ");
        }

        Console.WriteLine("\nExiting app");

        _exit = true;
    }

    /// <summary>
    /// Process that does some work on shared resources.
    /// </summary>
    private static void DoWork()
    {
        //int dur = _rand.Next(5000);
        int dur = 1000; //+ _rand.Next(10)*100;
        int n = 1;

        while ( !_exit )
        {
            _lock.WaitOne(); // Ensure only a given number of threads use resources.

            Thread.Sleep(dur);

            if ( _useMutex )
            {
                _mutex.WaitOne();
                _counter++;
                _steps = -_steps;
                _balance += _steps;
                Console.WriteLine("{0}: _count={1}; _balance={2}", Thread.CurrentThread.Name, _counter,_balance);
                _mutex.ReleaseMutex();
            }
            else
            {
                _counter++;
                _steps = -_steps;
                _balance += _steps;
                Console.WriteLine("{0}: _count={1}; _balance={2}", Thread.CurrentThread.Name, _counter, _balance);
            }

            _lock.Release();
            n++;
        }
    }
}