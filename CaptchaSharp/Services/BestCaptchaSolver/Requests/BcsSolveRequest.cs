using System;
using System.Linq;
using CaptchaSharp.Models;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Requests;

internal class BcsSolveRequest : BcsRequest
{
    [JsonProperty("cookie_input", NullValueHandling = NullValueHandling.Ignore)]
    public string? CookieInput { get; set; }
    
    [JsonProperty("user_agent", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserAgent { get; set; }
    
    [JsonProperty("proxy", NullValueHandling = NullValueHandling.Ignore)]
    public string? Proxy { get; set; }
    
    [JsonProperty("proxy_type", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyType { get; set; }

    public void SetProxy(Proxy? proxy)
    {
        if (proxy == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(proxy.UserAgent))
        {
            UserAgent = proxy.UserAgent;
        }

        if (proxy.Cookies is not null)
        {
            CookieInput = string.Join("; ", proxy.Cookies
                .Select(c => $"{c.Name}={c.Value}"));
        }
        
        // Only http(s) proxies are supported
        if (proxy.Type is not Enums.ProxyType.HTTP && proxy.Type is not Enums.ProxyType.HTTPS)
        {
            throw new NotSupportedException("Only HTTP and HTTPS proxies are supported");
        }

        if (string.IsNullOrEmpty(proxy.Host))
        {
            return;
        }
        
        Proxy = proxy.RequiresAuthentication
            ? $"{proxy.Username}:{proxy.Password}@{proxy.Host}:{proxy.Port}"
            : $"{proxy.Host}:{proxy.Port}";

        ProxyType = "HTTP";
    }
}
