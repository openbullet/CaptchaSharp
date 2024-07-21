namespace CaptchaSharp.Models.CapSolver.Responses.Solutions;

internal class FuncaptchaSolution : Solution
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
