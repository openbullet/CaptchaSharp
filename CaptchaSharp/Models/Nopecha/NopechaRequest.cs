using Newtonsoft.Json;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaRequest
{
    [JsonProperty("key")]
    public required string ApiKey { get; set; }
}
