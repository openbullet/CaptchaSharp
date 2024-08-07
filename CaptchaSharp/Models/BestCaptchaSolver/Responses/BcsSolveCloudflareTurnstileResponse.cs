using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsSolveCloudflareTurnstileResponse : BcsResponse
{
    [JsonProperty("solution")]
    public string? Solution { get; set; }
    
    [JsonProperty("user_agent")]
    public string? UserAgent { get; set; }
}
