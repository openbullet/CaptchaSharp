using CaptchaSharp.Models.CaptchaResponses;

namespace CaptchaSharp.Models.SolveCaptcha.Responses.Solutions;

internal class GeeTestSolveCaptchaTaskSolution : SolveCaptchaTaskSolution
{
    public string? Challenge { get; set; }
    public string? Validate { get; set; }
    public string? SecCode { get; set; }

    public override CaptchaResponse ToCaptchaResponse(string id)
    {
        return new GeeTestResponse
        {
            Id = id,
            Challenge = Challenge!,
            Validate = Validate!,
            SecCode = SecCode!
        };
    }
}
