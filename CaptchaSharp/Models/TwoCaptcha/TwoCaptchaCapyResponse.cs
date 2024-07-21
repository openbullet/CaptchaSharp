using CaptchaSharp.Models;

namespace CaptchaSharp.Models.TwoCaptcha;

internal class TwoCaptchaCapyResponse : TwoCaptchaResponse
{
    public new CapySolution? Request { get; set; }
}

internal class CapySolution
{
    public string? CaptchaKey { get; set; }
    public string? ChallengeKey { get; set; }
    public string? Answer { get; set; }

    public CapyResponse ToCapyResponse(string id)
    {
        return new CapyResponse
        {
            Id = id,
            CaptchaKey = CaptchaKey!,
            ChallengeKey = ChallengeKey!,
            Answer = Answer!
        };
    }
}
