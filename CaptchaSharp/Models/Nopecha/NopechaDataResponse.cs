using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaDataResponse : NopechaResponse
{
    [JsonProperty("data")]
    public JToken? Data { get; set; }
}
