using System;
using System.Linq;
using CaptchaSharp.Models;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaSolveTokenRequest : NopechaSolveRequest
{
    [JsonProperty("proxy", NullValueHandling = NullValueHandling.Ignore)]
    public NopechaProxy? Proxy { get; set; }
    
    [JsonProperty("cookie", NullValueHandling = NullValueHandling.Ignore)]
    public NopechaCookie[]? Cookies { get; set; }
    
    [JsonProperty("useragent", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserAgent { get; set; }

    public NopechaSolveTokenRequest WithSessionParams(
        SessionParams? sessionParams, string url)
    {
        if (sessionParams is null)
        {
            return this;
        }

        UserAgent = sessionParams.UserAgent;
        
        var proxy = sessionParams.Proxy;
        
        if (proxy is null)
        {
            return this;
        }

        if (!string.IsNullOrEmpty(proxy.Host))
        {
            Proxy = new NopechaProxy
            {
                Scheme = proxy.Type.ToString().ToLower(),
                Host = proxy.Host,
                Port = proxy.Port,
                Username = proxy.Username,
                Password = proxy.Password
            };
        }

        if (sessionParams.Cookies is null)
        {
            return this;
        }
        
        var uri = new Uri(url);
            
        Cookies = sessionParams.Cookies.Select(c => new NopechaCookie
        {
            Name = c.Key,
            Value = c.Value,
            Domain = uri.Host,
            Path = uri.AbsolutePath,
            Secure = uri.Scheme == "https",
                
            // Hardcoded since we don't have access to these values
            HostOnly = true,
            HttpOnly = true,
            Session = false,
            ExpirationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600
        }).ToArray();

        return this;
    }
}
