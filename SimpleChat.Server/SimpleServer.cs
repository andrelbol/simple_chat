using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Server
{
    public class SimpleServer
    {
        private readonly TcpListener _listener;
        private readonly ClientManager _manager;

        public SimpleServer(string host, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(host), port);
            _manager = new ClientManager();
        }

        public void StartListening()
        {
            _listener.Start();
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                _manager.AddClient(client);
            }
        }
    }
}
