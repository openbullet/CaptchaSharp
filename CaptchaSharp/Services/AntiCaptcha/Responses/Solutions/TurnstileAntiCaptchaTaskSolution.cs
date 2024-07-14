using CaptchaSharp.Models;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions;

internal class TurnstileAntiCaptchaTaskSolution : AntiCaptchaTaskSolution
{
    public required string Token { get; set; }

    public required string UserAgent { get; set; }
    
    public override CaptchaResponse ToCaptchaResponse(long id)
    {
        return new CloudflareTurnstileResponse
        {
            Id = id,
            Response = Token,
            UserAgent = UserAgent
        };
    }
}
