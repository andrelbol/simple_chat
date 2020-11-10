using SimpleChat.Commons.Extensions;
using System;
using System.Net.Sockets;
using System.Text;

namespace SimpleChat.Server.Models
{
    public class Client: IClient
    {
        private TcpClient _tcpClient { get; set; }
        private NetworkStream _stream { get; set; }
        public string Nickname { get; set; }
        public bool IsClosed { get; set; }
        public Room Room { get; set; }

        public Client(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
        }

        public void Write(string message)
            => _stream.Write(Encoding.ASCII.GetBytes($"{message}{Environment.NewLine}"));

        public string Read()
        {
            var bytes = new byte[1024];
            string message = "";
            while (string.IsNullOrEmpty(message))
            {
                _stream.Read(bytes);
                message = Encoding.ASCII.GetString(bytes).TrimNewLines();
            }
            return message;
        }

        public void Close()
        {
            _stream.Close();
            _tcpClient.Close();
            IsClosed = true;
        }
    }
}
