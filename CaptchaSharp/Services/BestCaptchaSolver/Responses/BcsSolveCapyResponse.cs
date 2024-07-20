using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Responses;

internal class BcsSolveCapyResponse : BcsResponse
{
    [JsonProperty("solution")]
    public string? Solution { get; set; }
}
