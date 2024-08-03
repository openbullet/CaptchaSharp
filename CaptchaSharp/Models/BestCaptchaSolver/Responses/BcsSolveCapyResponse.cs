using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsSolveCapyResponse : BcsResponse
{
    [JsonProperty("solution")]
    public string? Solution { get; set; }
}
