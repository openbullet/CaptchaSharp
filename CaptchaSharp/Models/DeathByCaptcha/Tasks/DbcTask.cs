using System;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks;

internal class DbcTask
{
    [JsonProperty("proxy", NullValueHandling = NullValueHandling.Ignore)]
    public string? Proxy { get; set; }
    
    [JsonProperty("proxytype", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyType { get; set; }

    public DbcTask SetProxy(Proxy? proxy)
    {
        if (proxy?.Host is null)
        {
            return this;
        }
        
        if (proxy.Type is not Enums.ProxyType.HTTP && proxy.Type is not Enums.ProxyType.HTTPS)
        {
            throw new NotSupportedException("DBC only supports HTTP proxies");
        }
        
        ProxyType = "HTTP";

        Proxy = proxy.RequiresAuthentication 
            ? $"http://{proxy.Username}:{proxy.Password}@{proxy.Host}:{proxy.Port}"
            : $"http://{proxy.Host}:{proxy.Port}";

        return this;
    }
}
