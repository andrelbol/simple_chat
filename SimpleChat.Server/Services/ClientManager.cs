using SimpleChat.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleChat.Server.Services
{
    public class ClientManager
    {
        private ConcurrentDictionary<string, Room> _rooms;
        private Room DefaultRoom;

        public ClientManager()
        {
            DefaultRoom = new Room("general");
            _rooms = new ConcurrentDictionary<string, Room>();
            _rooms.TryAdd(DefaultRoom.Name, DefaultRoom);
        }

        public void AddClient(TcpClient tcpClient)
        {
            var client = new Client(tcpClient);
            Task.Run(() => HandleClient(client));
        }

        private void HandleClient(Client client)
        {
            Console.WriteLine("Received connection");
            client.Nickname = RequestUsername(client);
            DefaultRoom.AddClient(client);
            StartClientCommunication(client);
        }

        private void StartClientCommunication(Client client)
        {
            while (!client.IsClosed)
            {
                var message = client.Read();
                MessageHandler.HandleMessage(message, client, _rooms);
            }
        }

        private string RequestUsername(Client client)
        {
            client.Write($"Welcome to our chat server.Please provide a nickname:{Environment.NewLine}>");
            var nickname = client.Read();
            var allClients = _rooms.Values.SelectMany(x => x.Clients.Keys);
            while (allClients.Contains(nickname))
            {
                client.Write($"Sorry, the nickname {nickname} is already taken. " +
                    $"Please choose a different one:{Environment.NewLine}>");
                nickname = client.Read();
            }
            client.Write($"*** You are registered as {nickname}. Joining room #{DefaultRoom.Name}.");
            return nickname;
        }
    }
}
