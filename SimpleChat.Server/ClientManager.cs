using SimpleChat.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleChat.Server
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
            while (true)
            {
                var message = client.Read();
                if (message.StartsWith("/p") || message.StartsWith("/u")) // Direct message
                {
                    SendDirectMessage(client, message);
                }
                else if (message.StartsWith("/room"))
                {
                    ChangeOrCreateRoom(client, message.Split(' ')[1]);
                }
                else if (message == "/exit") // Exit message
                {
                    ExitChat(client);
                    break;
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

        private void BroadcastMessage(Client client, string message)
        {
            var roomClients = client.Room.Clients.Values;
            foreach (var roomClient in roomClients)
            {
                roomClient.Write(message);
            }
        }

        private void SendDirectMessage(Client clientFrom, string message)
        {
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

        private void SendHelpMessage(Client client)
            => client.Write(Environment.NewLine +
                "=============================================" + Environment.NewLine +
                "| Public direct Message: /u <user> <message> |" + Environment.NewLine +
                "| Private Message: /p <user> <message>       |" + Environment.NewLine +
                "| Change room: /room <roomName>              |" + Environment.NewLine +
                "| Help: /help                                |" + Environment.NewLine +
                "| Exit: /exit                                |" + Environment.NewLine +
                "=============================================");

        private void ExitChat(Client client)
        {
            client.Write($"{client.Nickname} is exiting the chat");
            client.Close();
            client.Room.RemoveClient(client);
        }

        private void ChangeOrCreateRoom(Client client, string roomName)
        {
            if (_rooms.ContainsKey(roomName))
            {
                ChangeRoom(client, roomName);
            }
            else
            {
                CreateRoom(client, roomName);
            }
            client.Write($"You are now on room #{roomName}");
        }

        private void CreateRoom(Client client, string roomName)
        {
            var room = new Room(roomName);
            room.AddClient(client);
            _rooms.TryAdd(roomName, room);
        }

        private void ChangeRoom(Client client, string roomName)
        {
            _rooms.TryGetValue(roomName, out var room);
            room.AddClient(client);
        }
    }
}
