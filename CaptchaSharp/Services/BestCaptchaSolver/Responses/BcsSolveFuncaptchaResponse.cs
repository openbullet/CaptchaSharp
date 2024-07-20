using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Responses;

internal class BcsSolveFuncaptchaResponse : BcsResponse
{
    [JsonProperty("solution")]
    public string? Solution { get; set; }
}
