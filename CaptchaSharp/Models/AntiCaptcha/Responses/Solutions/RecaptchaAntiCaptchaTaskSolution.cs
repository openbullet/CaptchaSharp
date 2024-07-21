namespace CaptchaSharp.Models.AntiCaptcha.Responses.Solutions;

internal class RecaptchaAntiCaptchaTaskSolution : AntiCaptchaTaskSolution
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
