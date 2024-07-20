using CaptchaSharp.Models;
using System.Linq;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.CapSolver.Requests.Tasks.Proxied;

internal class CapSolverTask : CapSolverTaskProxyless
{
    public string? ProxyType { get; set; }
    public string? ProxyAddress { get; set; }
    public int ProxyPort { get; set; }
    
    [JsonProperty("proxyLogin", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyLogin { get; set; }
    
    [JsonProperty("proxyPassword", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyPassword { get; set; }
    
    [JsonProperty("userAgent", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserAgent { get; set; }
    
    [JsonProperty("cookies", NullValueHandling = NullValueHandling.Ignore)]
    public CapSolverCookie[]? Cookies { get; set; }

    public CapSolverTask SetProxy(Proxy proxy)
    {
        ProxyAddress = proxy.Host;
        ProxyPort = proxy.Port;
        ProxyType = proxy.Type.ToString().ToLower();
        ProxyLogin = proxy.Username;
        ProxyPassword = proxy.Password;
        UserAgent = proxy.UserAgent;

        if (proxy.Cookies is not null)
        {
            Cookies = proxy.Cookies.Select(c => new CapSolverCookie
            {
                Name = c.Name,
                Value = c.Value
            }).ToArray();
        }

        return this;
    }
}

internal class CapSolverCookie
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}
