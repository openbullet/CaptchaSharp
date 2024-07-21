using Newtonsoft.Json;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaResponse
{
    [JsonProperty("error")]
    public int? Error { get; set; }
    
    [JsonProperty("message")]
    public string? Message { get; set; }
    
    [JsonIgnore]
    public bool IsSuccess => Error is null;
}
