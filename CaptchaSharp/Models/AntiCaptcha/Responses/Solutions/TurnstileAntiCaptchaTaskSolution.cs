using CaptchaSharp.Models;

namespace CaptchaSharp.Models.AntiCaptcha.Responses.Solutions;

internal class TurnstileAntiCaptchaTaskSolution : AntiCaptchaTaskSolution
{
    public required string Token { get; set; }

    public required string UserAgent { get; set; }
    
    public override CaptchaResponse ToCaptchaResponse(string id)
    {
        return new CloudflareTurnstileResponse
        {
            Id = id,
            Response = Token,
            UserAgent = UserAgent
        };
    }
}
