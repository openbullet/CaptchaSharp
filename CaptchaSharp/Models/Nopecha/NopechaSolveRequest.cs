using Newtonsoft.Json;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaSolveRequest : NopechaRequest
{
    [JsonProperty("type")]
    public string Type { get; protected set; } = "";
}
