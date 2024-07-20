using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Requests;

internal class BcsSolveFuncaptchaRequest : BcsSolveRequest
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("s_url")]
    public required string SUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
}
