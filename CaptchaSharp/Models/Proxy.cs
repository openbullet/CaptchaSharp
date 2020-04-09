using CaptchaSharp.Enums;

namespace CaptchaSharp.Models
{
    public struct Proxy
    {
        public string Host;
        public int Port;
        public ProxyType Type;
        public bool RequiresAuthentication;
        public string Username;
        public string Password;

        public Proxy(string host, int port, ProxyType type = ProxyType.HTTPS)
        {
            Host = host;
            Port = port;
            Type = type;
            RequiresAuthentication = false;
            Username = "";
            Password = "";
        }

        public Proxy(string host, int port, string username, string password, ProxyType type = ProxyType.HTTPS)
        {
            Host = host;
            Port = port;
            Type = type;
            RequiresAuthentication = true;
            Username = username;
            Password = password;
        }
    }
}
