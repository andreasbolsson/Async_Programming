internal class Program
{


    private static void Main(string[] args)
    {
        int limit = Array.MaxLength;
        //int limit = 100000;

        Console.WriteLine("Calculating primes below {0}...\n", limit);
        bool[] numbers = new bool[limit];
        bool end = false;


        DateTime t = DateTime.Now;

        int x = 2;
        int n = 0;
        numbers[x] = true;
        
        while (x != -1)
        {
            x = MarkComposites(x);
            n++;
        }

        TimeSpan dur = DateTime.Now - t;
        Console.WriteLine("{0} prime numbers in total.", n.ToString("N0"));
        Console.WriteLine("Execution time: {0}", dur);

        /*
        Console.Write(@"{");

        for (x = 3; x < limit; x++)
        {
            if (numbers[x])
            {
                Console.Write("," + x);
            }
        }


        Console.WriteLine(@"}");
        */

        int MarkComposites(int prime) // Increments starting prime by the prime itself to figure out all number of which it is a composite.
        {
            int m = prime * 2;  //Calculate first composite following the primes itself.

            while (m < limit && m > 0)
            {
                numbers[m] = true;           
                m += prime;
            }

            int n = prime + 1; // Start at number right after prime.

            while (n < limit )
            {
                if (numbers[n] == false)
                {
                    //Console.Write("," + n);
                    return n;                        
                }

                n++;
            }
            

            return -1;
        }
    }

   
}