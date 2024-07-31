using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapSolver.Responses.Solutions;

internal class MtCaptchaSolution : Solution
{
    [JsonProperty("token")]
    public required string Token { get; set; }
    
    public override CaptchaResponse ToCaptchaResponse(string id)
    {
        return new StringResponse
        {
            Id = id,
            Response = Token
        };
    }
}
