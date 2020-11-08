using System;
using System.Collections.Concurrent;

namespace SimpleChat.Server
{
    public class Room
    {
        public ConcurrentDictionary<string, Client> Clients { get; set; }
        public string Name { get; set; }

        public Room(string name)
        {
            Name = name;
            Clients = new ConcurrentDictionary<string, Client>();
        }

        public void AddClient(Client client)
        {
            if (client.Room != null)
            {
                client.Room.RemoveClient(client);
            }
            Clients.TryAdd(client.Nickname, client);
            client.Room = this;
            Console.WriteLine($"User {client.Nickname} added to room #{Name}");
        }

        public void RemoveClient(Client client)
            => Clients.TryRemove(client.Nickname, out var _);
    }
}
