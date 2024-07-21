using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsSolveImageResponse : BcsResponse
{
    [JsonProperty("text")]
    public string? Text { get; set; }
}
