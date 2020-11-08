using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Client
{
    public class SimpleClient
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public SimpleClient(string host, int port)
        {
            _client = new TcpClient(host, port);
            _stream = _client.GetStream();
        }

        public void Start()
        {
            var writing = Task.Run(() => StartWriting());
            var reading = Task.Run(() => StartReading());
            Task.WaitAll(writing, reading);
            _stream.Close();
            _client.Close();
        }

        private void StartWriting()
        {
            while (true)
            {
                var message = $"{Console.ReadLine()}{Environment.NewLine}";
                var byteMessage = Encoding.ASCII.GetBytes(message);
                _stream.Write(byteMessage);
            }
        }

        private void StartReading()
        {
            while (true)
            {
                var byteMessage = new byte[1024];
                _stream.Read(byteMessage);
                var message = Encoding.ASCII.GetString(byteMessage)
                    .Replace(Environment.NewLine, "")
                    .Replace("\0", "");
                Console.WriteLine(message);
            }
        }
    }
}
