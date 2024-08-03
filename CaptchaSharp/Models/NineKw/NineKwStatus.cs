using Newtonsoft.Json;

namespace CaptchaSharp.Models.NineKw;

internal class NineKwStatus
{
    [JsonProperty("https")]
    public required int Https { get; set; }
    
    [JsonProperty("success")]
    public required bool Success { get; set; }
}
