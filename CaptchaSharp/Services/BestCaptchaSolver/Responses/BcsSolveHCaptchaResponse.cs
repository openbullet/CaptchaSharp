using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Responses;

internal class BcsSolveHCaptchaResponse : BcsResponse
{
    [JsonProperty("solution")]
    public string? Solution { get; set; }
}
