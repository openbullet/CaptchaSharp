using CaptchaSharp.Enums;
using Newtonsoft.Json;

namespace CaptchaSharp.Models;

/// <summary>A generic proxy class.</summary>
public class Proxy
{
    /// <summary></summary>
    public required string Host { get; set; }

    /// <summary></summary>
    public required int Port { get; set; }

    /// <summary></summary>
    public ProxyType Type { get; set; } = ProxyType.HTTP;

    /// <summary></summary>
    public string? Username { get; set; }

    /// <summary></summary>
    public string? Password { get; set; }

    /// <summary>Whether the proxy requires authentication.</summary>
    [JsonIgnore]
    public bool RequiresAuthentication => !string.IsNullOrEmpty(Username);

    /// <summary></summary>
    public Proxy()
    {
        // This parameterless constructor is required for JSON deserialization.
    }

    /// <summary></summary>
    public Proxy(string host, int port, ProxyType type = ProxyType.HTTP,
        string? username = null, string? password = null)
    {
        Host = host;
        Port = port;
        Type = type;
        Username = username;
        Password = password;
    }
}
