using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied;

internal class HCaptchaDbcTask : DbcTask
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
