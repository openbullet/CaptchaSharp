using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Requests;

internal class BcsSolveHCaptchaRequest : BcsSolveRequest
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
}
