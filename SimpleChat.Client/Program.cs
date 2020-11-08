using System;
using System.Net.Sockets;

namespace SimpleChat.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int port = 20000;
            try
            {
                Console.WriteLine($"Connecting to server on address: {host}:{port}");
                var client = new SimpleClient(host, port);
                client.Start();
            }
            catch (SocketException)
            {
                Console.WriteLine("Connection error. Verify if server is running on same host:port.");
                Console.ReadLine();
            }
        }
    }
}
