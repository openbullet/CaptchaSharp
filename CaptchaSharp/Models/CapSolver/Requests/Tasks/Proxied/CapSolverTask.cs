using System.Linq;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;

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

    public CapSolverTask WithSessionParams(SessionParams sessionParams)
    {
        UserAgent = sessionParams.UserAgent;

        if (sessionParams.Cookies is not null)
        {
            Cookies = sessionParams.Cookies.Select(c => new CapSolverCookie
            {
                Name = c.Key,
                Value = c.Value
            }).ToArray();
        }
        
        var proxy = sessionParams.Proxy;
        
        if (proxy is null)
        {
            return this;
        }
        
        ProxyAddress = proxy.Host;
        ProxyPort = proxy.Port;
        ProxyType = proxy.Type.ToString().ToLower();
        ProxyLogin = proxy.Username;
        ProxyPassword = proxy.Password;

        return this;
    }
}

internal class CapSolverCookie
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}
