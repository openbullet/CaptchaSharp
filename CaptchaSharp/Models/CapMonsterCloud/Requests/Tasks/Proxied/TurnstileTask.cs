using CaptchaSharp.Models.AntiCaptcha.Requests.Tasks.Proxied;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapMonsterCloud.Requests.Tasks.Proxied;

internal class TurnstileTask : AntiCaptchaTask
{
    [JsonProperty("websiteURL")]
    public required string WebsiteUrl { get; set; }
    
    [JsonProperty("websiteKey")]
    public required string WebsiteKey { get; set; }
    
    [JsonProperty("cloudflareTaskType")]
    public string CloudflareTaskType { get; set; } = "cf_clearance";
    
    [JsonProperty("htmlPageBase64")]
    public required string HtmlPageBase64 { get; set; }
    
    public TurnstileTask()
    {
        Type = "TurnstileTask";
    }
}
