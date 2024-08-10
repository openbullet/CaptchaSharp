using CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapMonsterCloud.Requests.Tasks;

internal class TurnstileTaskProxyless : AntiCaptchaTaskProxyless
{
    [JsonProperty("websiteURL")]
    public required string WebsiteUrl { get; set; }
    
    [JsonProperty("websiteKey")]
    public required string WebsiteKey { get; set; }
    
    [JsonProperty("pageAction", NullValueHandling = NullValueHandling.Ignore)]
    public string? PageAction { get; set; }
    
    [JsonProperty("cloudflareTaskType")]
    public string CloudflareTaskType { get; set; } = "token";
    
    [JsonProperty("userAgent", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserAgent { get; set; }
    
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public string? CData { get; set; }
    
    [JsonProperty("pageData", NullValueHandling = NullValueHandling.Ignore)]
    public string? PageData { get; set; }

    public TurnstileTaskProxyless()
    {
        Type = "TurnstileTaskProxyless";
    }
}
