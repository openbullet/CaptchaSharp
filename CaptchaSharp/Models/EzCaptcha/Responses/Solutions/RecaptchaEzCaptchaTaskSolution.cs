using CaptchaSharp.Models.CaptchaResponses;

namespace CaptchaSharp.Models.EzCaptcha.Responses.Solutions;

internal class RecaptchaEzCaptchaTaskSolution : EzCaptchaTaskSolution
{
    public string? GRecaptchaResponse { get; set; }

    public override CaptchaResponse ToCaptchaResponse(string id)
    {
        return new StringResponse
        {
            Id = id,
            Response = GRecaptchaResponse!
        };
    }
}
