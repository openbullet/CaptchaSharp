using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks;

internal class MtCaptchaDbcTaskProxyless : DbcTaskProxyless
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
