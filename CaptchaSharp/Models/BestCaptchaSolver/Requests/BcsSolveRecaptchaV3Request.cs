using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Requests;

internal class BcsSolveRecaptchaV3Request : BcsSolveRequest
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }
    
    [JsonProperty("v3_action", NullValueHandling = NullValueHandling.Ignore)]
    public string? Action { get; set; }
    
    [JsonProperty("v3_min_score", NullValueHandling = NullValueHandling.Ignore)]
    public double? MinScore { get; set; }
}
