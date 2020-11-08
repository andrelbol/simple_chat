using System;
using System.Net.Sockets;

namespace SimpleChat.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int port = GetPort();
            try
            {
                var client = new SimpleClient(host, port);
                client.Start();
            }
            catch (SocketException)
            {
                Console.WriteLine("Connection error. Verify if server is running on same host:port.");
                Console.ReadLine();
            }
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
