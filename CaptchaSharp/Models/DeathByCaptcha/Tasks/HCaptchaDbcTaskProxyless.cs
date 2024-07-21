using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks;

internal class HCaptchaDbcTaskProxyless : DbcTaskProxyless
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}