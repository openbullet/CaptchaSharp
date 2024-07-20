using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Responses;

internal class BcsSolveImageResponse : BcsResponse
{
    [JsonProperty("text")]
    public string? Text { get; set; }
}
