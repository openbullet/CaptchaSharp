using CaptchaSharp.Models;
using System;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied;

internal class DbcTask : DbcTaskProxyless
{
    [JsonProperty("proxy")]
    public string? Proxy { get; set; }
    
    [JsonProperty("proxytype")]
    public string ProxyType { get; set; } = "HTTP";

    public DbcTask SetProxy(Proxy proxy)
    {
        if (proxy.Type is not Enums.ProxyType.HTTP && proxy.Type is not Enums.ProxyType.HTTPS)
        {
            throw new NotSupportedException("DBC only supports HTTP proxies");
        }

        Proxy = proxy.RequiresAuthentication 
            ? $"http://{proxy.Username}:{proxy.Password}@{proxy.Host}:{proxy.Port}"
            : $"http://{proxy.Host}:{proxy.Port}";

        return this;
    }
}
