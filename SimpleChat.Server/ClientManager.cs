﻿using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleChat.Server
{
    public class ClientManager
    {
        private ConcurrentBag<Client> _clients;

        public ClientManager()
        {
            _clients = new ConcurrentBag<Client>();
        }

        public void AddClient(TcpClient tcpClient)
        {
            var client = new Client(tcpClient);
            _clients.Add(client);
            Task.Run(() => HandleClient(client));
        }

        private void HandleClient(Client client)
        {
            Console.WriteLine("Client conectado.");

            while (true)
            {
                BroadcastMessage(client.Read());
            }
        }

        private void BroadcastMessage(string message)
        {
            foreach (var client in _clients)
            {
                client.Write(message);
            }
        }
    }
}
