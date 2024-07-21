using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks.Proxied;

internal class CloudflareTurnstileDbcTask : DbcTask
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("action")]
    public string? Action { get; set; }
}
