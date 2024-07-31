using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks.Proxied;

internal class FriendlyCaptchaDbcTask : DbcTask
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
