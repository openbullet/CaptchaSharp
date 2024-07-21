using CaptchaSharp.Models;

namespace CaptchaSharp.Services.CapSolver.Responses.Solutions;

internal class CloudflareTurnstileSolution : Solution
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
