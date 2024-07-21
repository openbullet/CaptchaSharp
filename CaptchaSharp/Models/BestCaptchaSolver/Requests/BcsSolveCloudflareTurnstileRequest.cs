using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Requests;

internal class BcsSolveCloudflareTurnstileRequest : BcsSolveRequest
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
    public string? Action { get; set; }
    
    [JsonProperty("cdata", NullValueHandling = NullValueHandling.Ignore)]
    public string? CData { get; set; }
}
