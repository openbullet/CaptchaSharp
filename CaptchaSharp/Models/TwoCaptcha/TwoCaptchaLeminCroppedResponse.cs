using Newtonsoft.Json;

namespace CaptchaSharp.Models.TwoCaptcha;

internal class TwoCaptchaLeminCroppedResponse : TwoCaptchaResponse
{
    public new LeminCroppedSolution? Request { get; set; }
}

internal class LeminCroppedSolution
{
    [JsonProperty("answer")]
    public required string Answer { get; set; }
    
    [JsonProperty("challenge_id")]
    public required string ChallengeId { get; set; }

    public LeminCroppedResponse ToLeminCroppedResponse(string id)
    {
        return new LeminCroppedResponse
        {
            Id = id,
            Answer = Answer,
            ChallengeId = ChallengeId
        };
    }
}
