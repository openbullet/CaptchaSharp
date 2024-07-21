using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsSolveHCaptchaResponse : BcsResponse
{
    [JsonProperty("solution")]
    public string? Solution { get; set; }
}
