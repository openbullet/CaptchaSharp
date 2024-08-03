using Newtonsoft.Json;

namespace CaptchaSharp.Models.NineKw;

internal class NineKwCheckResponse : NineKwResponse
{
    [JsonProperty("answer")]
    public required string Answer { get; set; }
    
    [JsonProperty("message")]
    public required string Message { get; set; }
    
    [JsonProperty("try_again")]
    public required int TryAgain { get; set; }
    
    [JsonProperty("timeout")]
    public required long Timeout { get; set; }
    
    [JsonProperty("credits")]
    public required long Credits { get; set; }
}
