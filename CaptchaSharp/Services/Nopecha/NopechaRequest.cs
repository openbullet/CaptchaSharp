using Newtonsoft.Json;

namespace CaptchaSharp.Services.Nopecha;

internal class NopechaRequest
{
    [JsonProperty("key")]
    public required string ApiKey { get; set; }
}
