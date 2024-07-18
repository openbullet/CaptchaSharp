using Newtonsoft.Json;

namespace CaptchaSharp.Services.NineKw;

internal class NineKwSubmitResponse : NineKwResponse
{
    [JsonProperty("captchaid")]
    public required string CaptchaId { get; set; }
}
