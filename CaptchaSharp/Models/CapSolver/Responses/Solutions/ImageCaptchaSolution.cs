using CaptchaSharp.Models;

namespace CaptchaSharp.Models.CapSolver.Responses.Solutions;

internal class ImageCaptchaSolution : Solution
{
    public string Text { get; set; }
    public string Url { get; set; }

    public override CaptchaResponse ToCaptchaResponse(string id)
    {
        return new StringResponse
        {
            Id = id,
            Response = Text
        };
    }
}