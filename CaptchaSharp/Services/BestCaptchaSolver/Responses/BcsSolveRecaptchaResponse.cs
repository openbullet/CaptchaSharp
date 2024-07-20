using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Responses;

internal class BcsSolveRecaptchaResponse : BcsResponse
{
    [JsonProperty("gresponse")]
    public string? GResponse { get; set; }
}
    
