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

    public void SetProxy(Proxy? proxy, string url)
    {
        if (proxy is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(proxy.UserAgent))
        {
            UserAgent = proxy.UserAgent;
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

        if (proxy.Cookies is not null)
        {
            var uri = new Uri(url);
            
            Cookies = proxy.Cookies.Select(c => new NopechaCookie
            {
                Name = c.Name,
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
        }
    }
}
