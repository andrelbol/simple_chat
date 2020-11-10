using SimpleChat.Server.Models;
using SimpleChat.Server.Services;
using SimpleChat.Tests.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xunit;

namespace SimpleChat.Tests
{
    public class MessageHandlerTests
    {
        public Dictionary<int, TestClient> Clients { get; set; }
        public ConcurrentDictionary<string, Room> Rooms { get; set; }
        public MessageHandlerTests()
        {
            Clients = new Dictionary<int, TestClient>();
            Rooms = new ConcurrentDictionary<string, Room>();
            var Room1 = new Room("#general1");
            var Room2 = new Room("#general2");

            Clients.Add(1, new TestClient("TestUser1"));
            Clients.Add(2, new TestClient("TestUser2"));
            Clients.Add(3, new TestClient("TestUser3"));
            
            Room1.AddClient(Clients.GetValueOrDefault(1));
            Room1.AddClient(Clients.GetValueOrDefault(2));
            Room2.AddClient(Clients.GetValueOrDefault(3));

            Rooms.TryAdd("#general1", Room1);
            Rooms.TryAdd("#general1", Room2);
        }


        [Theory]
        [InlineData("TestUser1 says privately to TestUser2: Hello", 1, 2)]
        [InlineData("TestUser2 says privately to TestUser1: Hello", 2, 1)]
        public void HandleMessage_ShouldSendPrivateMessage(string expected,
            int former, int latter)
        {
            var user1 = Clients.GetValueOrDefault(former);
            var user2 = Clients.GetValueOrDefault(latter);
            var message = "Hello";
            string command = $"/p {user2.Nickname} {message}";

            MessageHandler.HandleMessage(command, user1, Rooms);

            Assert.Equal(expected, user2.WrittenMessage);
        }

        [Fact]
        public void HandleMessage_ShouldNotSendPrivateMessageToOtherRoom()
        {
            var user1 = Clients.GetValueOrDefault(1);
            var user2 = Clients.GetValueOrDefault(3);
            var message = "Hello";
            string command = $"/p {user2.Nickname} {message}";

            MessageHandler.HandleMessage(command, user1, Rooms);

            Assert.Equal($"User {user2.Nickname} not found on room #{user1.Room.Name}.", 
                user1.WrittenMessage);
        }
    }
}
