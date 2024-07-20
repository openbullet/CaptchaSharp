using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Requests;

internal class BcsRequest
{
    [JsonProperty("access_token")]
    public required string AccessToken { get; set; }
    
    [JsonProperty("affiliate_id", NullValueHandling = NullValueHandling.Ignore)]
    public string? AffiliateId { get; set; }
}
