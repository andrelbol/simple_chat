using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SimpleChat.Commons.Extensions;

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
            Console.WriteLine("Connection finished.");
        }

        private void StartWriting()
        {
            while (_stream.CanWrite)
            {
                var message = $"{Console.ReadLine()}{Environment.NewLine}";
                var byteMessage = Encoding.ASCII.GetBytes(message);
                _stream.Write(byteMessage);
            }
        }

        private void StartReading()
        {
            var byteMessage = new byte[1024];
            while (_stream.Read(byteMessage) != 0)
            {
                var message = Encoding.ASCII.GetString(byteMessage).TrimNewLines();
                Console.WriteLine(message);
                byteMessage = new byte[1024];
            }
        }
    }
}
