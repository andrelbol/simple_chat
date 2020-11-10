using SimpleChat.Server.Models;

namespace SimpleChat.Tests.Models
{
    public class TestClient : IClient
    {
        public string Nickname { get; set; }
        public Room Room { get; set; }
        public string WrittenMessage { get; set; }
        public bool IsClosed { get; set; }

        public TestClient(string nickname) => Nickname = nickname;

        public void Close() => IsClosed = true;

        public string Read() => null;

        public void Write(string message) => WrittenMessage = message;
    }
}
