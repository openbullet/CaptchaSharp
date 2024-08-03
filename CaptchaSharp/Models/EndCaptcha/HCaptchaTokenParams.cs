using Newtonsoft.Json;

namespace CaptchaSharp.Models.EndCaptcha;

internal class HCaptchaTokenParams : EndCaptchaTokenParams
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
