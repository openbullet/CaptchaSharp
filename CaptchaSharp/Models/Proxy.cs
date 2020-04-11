using CaptchaSharp.Enums;

namespace CaptchaSharp.Models
{
    public class Proxy
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public ProxyType Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // Only supported by AntiCaptcha
        public string UserAgent { get; set; }
        public (string, string)[] Cookies { get; set; }

        public bool RequiresAuthentication => !string.IsNullOrEmpty(Username);

        public Proxy() { }

        public Proxy(string host, int port, ProxyType type = ProxyType.HTTP, string username = "", string password = "")
        {
            Host = host;
            Port = port;
            Type = type;
            Username = username;
            Password = password;
        }
    }
}
