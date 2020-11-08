using System;
using System.Net.Sockets;

namespace SimpleChat.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int port = 20000;

            try
            {
                var client = new SimpleClient(host, port);
                client.Start();
            }
            catch (SocketException e)
            {
                Console.WriteLine("Connection error. Verify if server is running on same host:port.");
                Console.ReadLine();
            }
        }
    }
}
