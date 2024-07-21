using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsTaskCreatedResponse : BcsResponse
{
    [JsonProperty("id")]
    public long Id { get; set; }
}
