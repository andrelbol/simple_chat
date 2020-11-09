using SimpleChat.Server.Models;
using System;
using System.Collections.Concurrent;

namespace SimpleChat.Server.Services
{
    public static class MessageHandler
    {
        public static void HandleMessage(string message, Client client,
            ConcurrentDictionary<string, Room> rooms)
        {
            if (message.StartsWith("/p") || message.StartsWith("/u")) // Direct message
            {
                SendDirectMessage(client, message);
            }
            else if (message.StartsWith("/room")) // Change room message
            {
                ChangeOrCreateRoom(client, message, rooms);
            }
            else if (message == "/exit") // Exit message
            {
                ExitChat(client);
            }
            else if (message == "/help") // Help message
            {
                SendHelpMessage(client);
            }
            else // Public message
            {
                BroadcastMessage(client, $"{client.Nickname}: {message}");
            }
        }

        private static void SendDirectMessage(Client clientFrom, string message)
        {
            if (message.Split(' ').Length <= 2)
            {
                SendInvalidParametersMessage(clientFrom);
                return;
            }
            var command = message.Split(' ')[0];
            var nickname = message.Split(' ')[1];
            var roomClients = clientFrom.Room.Clients;
            var clientFound = roomClients.TryGetValue(nickname, out var clientTo);
            if (!clientFound)
            {
                clientFrom.Write($"User {nickname} not found on room #{clientFrom.Room.Name}.");
            }
            else
            {
                const int FIRST_SPACE_INDEX = 3;
                var messageContent = message.Substring(
                    message.IndexOf(' ', FIRST_SPACE_INDEX) + 1);

                if (command == "/p")
                {
                    clientTo.Write($"{clientFrom.Nickname} says privately to {clientTo.Nickname}: " +
                        $"{messageContent}");
                }
                else
                {
                    BroadcastMessage(clientFrom, $"{clientFrom.Nickname} says to {clientTo.Nickname}: " +
                        $"{messageContent}");
                }
            }
        }

        private static void SendHelpMessage(Client client)
            => client.Write(Environment.NewLine +
                "=============================================" + Environment.NewLine +
                "| Public direct Message: /u <user> <message> |" + Environment.NewLine +
                "| Private Message: /p <user> <message>       |" + Environment.NewLine +
                "| Public Message: <message>                  |" + Environment.NewLine +
                "| Change room: /room <roomName>              |" + Environment.NewLine +
                "| Help: /help                                |" + Environment.NewLine +
                "| Exit: /exit                                |" + Environment.NewLine +
                "=============================================");

        private static void ExitChat(Client client)
        {
            client.Close();
            client.Room.RemoveClient(client);
            BroadcastMessage(client, $"{client.Nickname} is exiting the chat. ");
        }

        private static void ChangeOrCreateRoom(Client client, string message,
            ConcurrentDictionary<string, Room> rooms)
        {
            if (message.Split(' ').Length != 2)
            {
                SendInvalidParametersMessage(client);
                return;
            }
            string roomName = message.Split(' ')[1];
            if (rooms.ContainsKey(roomName))
            {
                ChangeRoom(client, roomName, rooms);
            }
            else
            {
                CreateRoom(client, roomName, rooms);
            }
            client.Write($"You are now on room #{roomName}");
        }

        private static void CreateRoom(Client client, string roomName,
            ConcurrentDictionary<string, Room> rooms)
        {
            var room = new Room(roomName);
            room.AddClient(client);
            rooms.TryAdd(roomName, room);
        }

        private static void ChangeRoom(Client client, string roomName,
            ConcurrentDictionary<string, Room> rooms)
        {
            rooms.TryGetValue(roomName, out var room);
            room.AddClient(client);
        }

        private static void BroadcastMessage(Client client, string message)
        {
            var roomClients = client.Room.Clients.Values;
            foreach (var roomClient in roomClients)
            {
                roomClient.Write(message);
            }
        }

        private static void SendInvalidParametersMessage(Client client)
            => client.Write("Invalid number of parameters. Consult /help to see usage.");
    }
}
