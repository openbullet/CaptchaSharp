using System;
using System.Linq;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Requests;

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

    public BcsSolveRequest WithSessionParams(SessionParams? sessionParams)
    {
        if (sessionParams is null)
        {
            return this;
        }

        UserAgent = sessionParams.UserAgent;

        if (sessionParams.Cookies is not null)
        {
            CookieInput = string.Join("; ", sessionParams.Cookies
                .Select(c => $"{c.Key}={c.Value}"));
        }
        
        var proxy = sessionParams.Proxy;
        
        if (proxy is null)
        {
            return this;
        }
        
        // Only http(s) proxies are supported
        if (proxy.Type is not Enums.ProxyType.HTTP && proxy.Type is not Enums.ProxyType.HTTPS)
        {
            throw new NotSupportedException("Only HTTP and HTTPS proxies are supported");
        }
        
        Proxy = proxy.RequiresAuthentication
            ? $"{proxy.Username}:{proxy.Password}@{proxy.Host}:{proxy.Port}"
            : $"{proxy.Host}:{proxy.Port}";

        ProxyType = "HTTP";
        
        return this;
    }
}
