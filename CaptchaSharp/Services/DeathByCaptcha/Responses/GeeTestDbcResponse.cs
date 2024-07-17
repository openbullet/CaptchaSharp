using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Responses;

internal class GeeTestDbcResponse
{
    [JsonProperty("challenge")]
    public required string Challenge { get; set; }
    
    [JsonProperty("validate")]
    public required string Validate { get; set; }
    
    [JsonProperty("seccode")]
    public required string Seccode { get; set; }
}
