namespace CaptchaSharp.Models.SolveCaptcha.Responses.Solutions;

internal class FuncaptchaSolveCaptchaTaskSolution : SolveCaptchaTaskSolution
{
    public string? Token { get; set; }

    public override CaptchaResponse ToCaptchaResponse(string id)
    {
        return new StringResponse
        {
            Id = id,
            Response = Token!
        };
    }
}
