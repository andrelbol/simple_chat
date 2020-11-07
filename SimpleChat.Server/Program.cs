using System;

namespace SimpleChat.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int port = 20000;
            var server = new SimpleServer(host, port);

            Console.WriteLine($"Servidor iniciado no endereço {host}:{port}.");
            server.StartListening();

        }
    }
}
