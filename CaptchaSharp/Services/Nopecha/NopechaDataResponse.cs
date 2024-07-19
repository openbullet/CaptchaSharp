using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services.Nopecha;

internal class NopechaDataResponse : NopechaResponse
{
    [JsonProperty("data")]
    public JToken? Data { get; set; }
}
