﻿using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleChat.Server
{
    public class ClientManager
    {
        private ConcurrentDictionary<string, Client> _clients;

        public ClientManager()
        {
            _clients = new ConcurrentDictionary<string, Client>();
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
            _clients.TryAdd(client.Nickname, client);

            while (true)
            {
                var message = client.Read();
                if (message.StartsWith("/p"))
                {
                    SendPrivateMessage(client, message);
                }
                else if (message == "exit")
                {
                    client.Write($"{client.Nickname} is exiting the room");
                    CloseClient(client);
                    break;
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
            while (_clients.ContainsKey(nickname))
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
            foreach (var client in _clients.Values)
            {
                client.Write(message);
            }
        }

        private void SendPrivateMessage(Client from, string message)
        {
            var nickname = message.Split(' ')[1];
            var clientFound = _clients.TryGetValue(nickname, out var to);
            if (!clientFound)
            {
                from.Write($"User {nickname} not found on the chat room.");
            }
            else
            {
                const int FIRST_SPACE_INDEX = 3;
                var messageContent = message.Substring(
                    message.IndexOf(' ', FIRST_SPACE_INDEX) + 1);

                to.Write($"{from.Nickname} says privately to {to.Nickname}: " +
                    $"{messageContent}");
            }
        }

        private void CloseClient(Client client)
        {
            client.Close();
            _clients.TryRemove(client.Nickname, out var _);
        }
    }
}
