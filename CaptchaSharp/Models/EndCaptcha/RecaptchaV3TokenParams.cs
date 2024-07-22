using Newtonsoft.Json;

namespace CaptchaSharp.Models.EndCaptcha;

internal class RecaptchaV3TokenParams : EndCaptchaTokenParams
{
    [JsonProperty("googlekey")]
    public required string GoogleKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("action")]
    public required string Action { get; set; }
    
    [JsonProperty("min_score")]
    public required double MinScore { get; set; }
}
