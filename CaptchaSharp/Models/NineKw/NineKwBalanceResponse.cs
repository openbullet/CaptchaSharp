using Newtonsoft.Json;

namespace CaptchaSharp.Models.NineKw;

internal class NineKwBalanceResponse : NineKwResponse
{   
    [JsonProperty("message")]
    public required string Message { get; set; }
    
    [JsonProperty("credits")]
    public required long Credits { get; set; }
}
