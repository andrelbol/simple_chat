using System;
using System.Net.Sockets;
using System.Text;
using SimpleChat.Commons.Extensions;

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
        }
    }
}
