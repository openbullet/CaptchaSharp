using System;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks.Proxied;

internal class SolveCaptchaTask : SolveCaptchaTaskProxyless
{
    [JsonProperty("proxy_type", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyType { get; set; }
    
    [JsonProperty("proxy_address", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyAddress { get; set; }
    
    [JsonProperty("proxy_port", NullValueHandling = NullValueHandling.Ignore)]
    public int? ProxyPort { get; set; }
    
    [JsonProperty("proxy_login", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyLogin { get; set; }
    
    [JsonProperty("proxy_password", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyPassword { get; set; }

    public override void SetParamsFromProxy(Proxy? proxy)
    {
        base.SetParamsFromProxy(proxy);
        
        if (proxy is null || string.IsNullOrEmpty(proxy.Host))
        {
            return;
        }
        
        if (!System.Net.IPAddress.TryParse(proxy.Host, out _))
        {
            throw new NotSupportedException(
                "Only IP addresses are supported for the proxy host");   
        }
        
        ProxyAddress = proxy.Host;
        ProxyPort = proxy.Port;
        ProxyType = proxy.Type.ToString().ToLower();
        ProxyLogin = proxy.Username;
        ProxyPassword = proxy.Password;
    }
}
