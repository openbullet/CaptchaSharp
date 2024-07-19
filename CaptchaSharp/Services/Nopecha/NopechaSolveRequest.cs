using Newtonsoft.Json;

namespace CaptchaSharp.Services.Nopecha;

internal class NopechaSolveRequest : NopechaRequest
{
    [JsonProperty("type")]
    public string Type { get; protected set; } = "";
}
