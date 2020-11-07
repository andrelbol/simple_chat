using System;
using System.Collections.Concurrent;
using System.Linq;
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
            Task.Run(() => HandleClient(client));
        }

        private void HandleClient(Client client)
        {
            Console.WriteLine("Client conectado.");
            client.Nickname = RequestUsername(client);
            _clients.Add(client);

            while (true)
            {
                var message = client.Read();
                if (message.StartsWith("/p"))
                {
                    SendPrivateMessage(client, message);
                }
                else
                {
                    BroadcastMessage($"{client.Nickname}> {message}");
                }
            }
        }
        private string RequestUsername(Client client)
        {
            client.Write($"Welcome to our chat server.Please provide a nickname:{Environment.NewLine}>");
            var nickname = client.Read();
            while (_clients.Any(x => x.Nickname == nickname))
            {
                client.Write($"Sorry, the nickname {nickname} is already taken. " +
                    $"Please choose a different one:{Environment.NewLine}>");
                nickname = client.Read();
            }
            client.Write($"*** You are registered as {nickname}. Joining room.");
            return nickname;
        }

        private void BroadcastMessage(string message)
        {
            foreach (var client in _clients)
            {
                client.Write(message);
            }
        }

        private void SendPrivateMessage(Client from, string message)
        {
            var nickname = message.Split(' ')[1];
            var to = _clients.FirstOrDefault(x => x.Nickname.Equals(nickname));
            if (to == null)
            {
                from.Write($"User {nickname} not found on the chat room.");
            }
            else
            {
                to.Write($"{from.Nickname} says privately to {to.Nickname} : {message.Split(' ')[1]}");
            }
        }
    }
}
