namespace SimpleChat.Server.Models
{
    public interface IClient
    {
        string Nickname { get; set; }
        Room Room { get; set; }
        void Write(string message);
        string Read();
        void Close();
    }
}
