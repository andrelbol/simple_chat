using System;
using System.Net.Sockets;
using System.Text;

namespace SimpleChat.Server
{
    public class Client
    {
        private TcpClient _tcpClient { get; set; }
        private NetworkStream _stream { get; set; }
        public string Nickname { get; set; }

        public Client(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
        }

        public void Write(string message)
            => _stream.Write(Encoding.ASCII.GetBytes(message));

        public string Read()
        {
            var bytes = new Byte[256];
            _stream.Read(bytes);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
