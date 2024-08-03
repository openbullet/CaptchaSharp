using CaptchaSharp.Models.CaptchaResponses;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapSolver.Responses.Solutions;

internal class AmazonWafSolution : Solution
{
    [JsonProperty("cookie")]
    public required string Cookie { get; set; }

    public override CaptchaResponse ToCaptchaResponse(string id)
    {
        return new StringResponse
        {
            Id = id,
            Response = Cookie
        };
    }
}
