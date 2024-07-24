using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Responses;

internal class LeminCroppedDbcResponse
{
    [JsonProperty("answer")]
    public required string Answer { get; set; }
    
    [JsonProperty("challengeid")]
    public required string ChallengeId { get; set; }
}
