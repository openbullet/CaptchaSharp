using Newtonsoft.Json;

namespace CaptchaSharp.Models.EndCaptcha;

internal class EndCaptchaTokenParams
{
    [JsonProperty("proxy", NullValueHandling = NullValueHandling.Ignore)]
    public string? Proxy { get; set; }
    
    [JsonProperty("proxytype", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyType { get; set; }

    public void SetProxy(Proxy? proxy)
    {
        if (proxy?.Host is null)
        {
            return;
        }
        
        Proxy = $"{proxy.Type.ToString().ToLower()}://{proxy.Host}:{proxy.Port}";
        ProxyType = proxy.Type.ToString().ToUpper();
    }
}
