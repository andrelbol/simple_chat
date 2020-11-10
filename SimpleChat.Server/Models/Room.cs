using System;
using System.Collections.Concurrent;

namespace SimpleChat.Server.Models
{
    public class Room
    {
        public ConcurrentDictionary<string, IClient> Clients { get; set; }
        public string Name { get; set; }

        public Room(string name)
        {
            Name = name;
            Clients = new ConcurrentDictionary<string, IClient>();
        }

        public void AddClient(IClient client)
        {
            if (client.Room != null)
            {
                client.Room.RemoveClient(client);
            }
            Clients.TryAdd(client.Nickname, client);
            client.Room = this;
            Console.WriteLine($"User {client.Nickname} added to room #{Name}");
        }

        public void RemoveClient(IClient client)
            => Clients.TryRemove(client.Nickname, out var _);
    }
}
