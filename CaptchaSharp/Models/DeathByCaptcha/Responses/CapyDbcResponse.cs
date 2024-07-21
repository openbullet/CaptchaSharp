using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Responses;

internal class CapyDbcResponse
{
    [JsonProperty("captchakey")]
    public required string CaptchaKey { get; set; }
    
    [JsonProperty("challengekey")]
    public required string ChallengeKey { get; set; }
    
    [JsonProperty("answer")]
    public required string Answer { get; set; }
}
