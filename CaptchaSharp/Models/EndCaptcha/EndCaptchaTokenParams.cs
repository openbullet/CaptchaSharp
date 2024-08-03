using Newtonsoft.Json;

namespace CaptchaSharp.Models.EndCaptcha;

internal class EndCaptchaTokenParams
{
    [JsonProperty("proxy", NullValueHandling = NullValueHandling.Ignore)]
    public string? Proxy { get; set; }
    
    [JsonProperty("proxytype", NullValueHandling = NullValueHandling.Ignore)]
    public string? ProxyType { get; set; }

    public EndCaptchaTokenParams WithSessionParams(SessionParams? sessionParams)
    {
        if (sessionParams?.Proxy is null)
        {
            return this;
        }
        
        var proxy = sessionParams.Proxy;
        
        Proxy = $"{proxy.Type.ToString().ToLower()}://{proxy.Host}:{proxy.Port}";
        ProxyType = proxy.Type.ToString().ToUpper();
        
        return this;
    }
}
