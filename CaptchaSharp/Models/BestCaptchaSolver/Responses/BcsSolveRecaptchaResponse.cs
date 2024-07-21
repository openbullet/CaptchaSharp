using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsSolveRecaptchaResponse : BcsResponse
{
    [JsonProperty("gresponse")]
    public string? GResponse { get; set; }
}
    
