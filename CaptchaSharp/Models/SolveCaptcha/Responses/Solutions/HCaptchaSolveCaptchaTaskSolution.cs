using CaptchaSharp.Models.CaptchaResponses;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Responses.Solutions;

internal class HCaptchaSolveCaptchaTaskSolution : SolveCaptchaTaskSolution
{
    [JsonProperty("hCaptchaResponse")]
    public string? HCaptchaResponse { get; set; }
    
    public override CaptchaResponse ToCaptchaResponse(string id)
    {
        return new StringResponse
        {
            Id = id,
            Response = HCaptchaResponse!
        };
    }
}
