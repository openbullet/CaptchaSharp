using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsSolveFuncaptchaResponse : BcsResponse
{
    [JsonProperty("solution")]
    public string? Solution { get; set; }
}
