using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

internal class Program
{
    /// 
    /// <summary>
    /// This sample application shows various ways for how to synchronyze work in a multithreaded application.
    /// </summary>
    /// 
    private static Semaphore _pool; // A semaphore shared by all threads that limits the number of threads concurrently working on resouces. 
    private static Thread[] _threads;
    private static Random _rand = new Random();
    private static bool _exit = false;

    private static Mutex _mutex = new Mutex(); // Mutual exlusion synchonizer to limit work to a single thread.

    private static void Main(string[] args)
    {
        Console.WriteLine("Classic threading example with semaphore.");

        _exit = false;
        _pool = new Semaphore(0,3);
        _threads = new Thread[5];

        int i = 0;
        int len = _threads.Length;

        for(i=0; i<len; i++)
        {
            _threads[i] = new Thread(() => { DoWork(); });
            _threads[i].Name = i.ToString();    
            _threads[i].Start();
        }

        _pool.Release(2);
        Console.WriteLine("Threads started.");

        ConsoleKeyInfo kinfo = Console.ReadKey();

        while(kinfo.Key != ConsoleKey.X)
        {
            kinfo = Console.ReadKey();
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
        int dur = 1000;
        int n = 1;

        while ( !_exit )
        {
            _pool.WaitOne();

            Thread.Sleep(dur);
            Console.WriteLine("Thread {0} has executed {1} times", Thread.CurrentThread.Name, n);

            _pool.Release();
            n++;
        }

        Console.WriteLine("Exting thread {0}.", Thread.CurrentThread.Name);
    }
   
}