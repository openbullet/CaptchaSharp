using Newtonsoft.Json;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaStatusResponse : NopechaResponse
{
    [JsonProperty("plan")]
    public required string Plan { get; set; }
    
    [JsonProperty("credit")]
    public required double Credit { get; set; }
    
    [JsonProperty("quota")]
    public required double Quota { get; set; }
    
    [JsonProperty("duration")]
    public required long Duration { get; set; }
    
    [JsonProperty("lastreset")]
    public required long LastReset { get; set; }
}
