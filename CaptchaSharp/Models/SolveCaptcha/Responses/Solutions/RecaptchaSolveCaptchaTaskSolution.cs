using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Responses.Solutions;

internal class RecaptchaSolveCaptchaTaskSolution : SolveCaptchaTaskSolution
{
    [JsonProperty("gRecaptchaResponse")]
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
