using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Requests;

internal class BcsSolveRecaptchaV2Request : BcsSolveRequest
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }
    
    [JsonProperty("data_s")]
    public string? DataS { get; set; }
}
