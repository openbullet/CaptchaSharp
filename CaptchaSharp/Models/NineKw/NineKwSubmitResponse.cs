using Newtonsoft.Json;

namespace CaptchaSharp.Models.NineKw;

internal class NineKwSubmitResponse : NineKwResponse
{
    [JsonProperty("captchaid")]
    public required string CaptchaId { get; set; }
}
