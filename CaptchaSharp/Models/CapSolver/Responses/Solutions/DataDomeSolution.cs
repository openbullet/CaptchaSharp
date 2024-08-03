using CaptchaSharp.Models.CaptchaResponses;

namespace CaptchaSharp.Models.CapSolver.Responses.Solutions;

internal class DataDomeSolution : Solution
{
    public string? Cookie { get; set; }

    public override CaptchaResponse ToCaptchaResponse(string id)
    {
        return new StringResponse
        {
            Id = id,
            Response = Cookie!
        };
    }
}
