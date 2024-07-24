using Newtonsoft.Json;

namespace CaptchaSharp.Models.TwoCaptcha;

internal class TwoCaptchaCapyResponse : TwoCaptchaResponse
{
    public new CapySolution? Request { get; set; }
}

internal class CapySolution
{
    [JsonProperty("captchakey")]
    public required string CaptchaKey { get; set; }
    
    [JsonProperty("challengekey")]
    public required string ChallengeKey { get; set; }
    
    [JsonProperty("answer")]
    public required string Answer { get; set; }

    public CapyResponse ToCapyResponse(string id)
    {
        return new CapyResponse
        {
            Id = id,
            CaptchaKey = CaptchaKey,
            ChallengeKey = ChallengeKey,
            Answer = Answer
        };
    }
}
