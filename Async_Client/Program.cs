using System.Net.Sockets;
using System.Net;

namespace Async_Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter message:");
            Console.ForegroundColor = ConsoleColor.White;

            string str = Console.ReadLine();

            while (str != String.Empty)
            {
                IPAddress addr;

                if (IPAddress.TryParse("127.0.0.1", out addr)) 
                {
                    IPEndPoint endPoint = new IPEndPoint(addr, 7089);
                    using (TcpClient client = new TcpClient())
                    {
                        client.Connect(endPoint);
                        NetworkStream stream = client.GetStream();

                        Byte[] b = System.Text.Encoding.UTF8.GetBytes(str);
                        

                        Span<byte> bspan = new Span<byte>(b);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Sending data...");

                        stream.Write(b);

                        Console.WriteLine("Sent.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to send data");
                }

                Console.WriteLine("\nEnter message:");
                str = Console.ReadLine();
            }
        }
    }
}