using System;

namespace SimpleChat.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int port = GetPort();
            var server = new SimpleServer(host, port);

            Console.WriteLine($"Server running on address {host}:{port}.");
            server.StartListening();
            Console.WriteLine("Server finished.");
        }

        static int GetPort()
        {
            Console.WriteLine("Choose port to connect to the server (default 20000):");
            var validPort = int.TryParse(Console.ReadLine(), out int port);
            if (!validPort)
            {
                Console.WriteLine("Port is not valid. Connecting on port 20000...");
                return 20000;
            }
            else
            {
                return port;
            }
        }
    }
}
