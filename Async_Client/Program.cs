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
                int amount = ReadInteger("\nEnter number of times: ",1);
                int port = ReadInteger("\nPort: ", 0);

                IPAddress addr;

                if (IPAddress.TryParse("127.0.0.1", out addr)) 
                {
                    IPEndPoint endPoint = new IPEndPoint(addr, port);
                    using (TcpClient client = new TcpClient())
                    {
                        client.Connect(endPoint);
                        NetworkStream stream = client.GetStream();

                        Byte[] b = System.Text.Encoding.UTF8.GetBytes(str);
                        
                        Span<byte> bspan = new Span<byte>(b); // For speed, safely allocated a contiguous area of memoty instead of using array stored in heap.

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Sending data...");

                        for (int i = 0; i < amount; i++) // Send message repeatedly the number of times defined by the user.
                        {
                            stream.Write(b); // Output the span of bytes that make up the message to the stream being sent to the local endpoint.
                        }

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

        private static int ReadInteger(string prompt, int min)
        {
            int number = 0;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(prompt);
            var pos = Console.GetCursorPosition();

            Console.ForegroundColor = ConsoleColor.White;

            while (!int.TryParse(Console.ReadLine(), out number) || number < min) // The number of times a message is sent must be once or more.
            {
                Console.SetCursorPosition(pos.Left, pos.Top);
                Console.WriteLine("                                          ");
                Console.SetCursorPosition(pos.Left, pos.Top);

                Console.ReadLine();
            }

            return number;
        }


    }
}