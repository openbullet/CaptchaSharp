using Newtonsoft.Json;

namespace CaptchaSharp.Models.Aycd.Responses;

internal class AycdAccessToken
{
    [JsonProperty("token")]
    public required string Token { get; set; }
    
    [JsonProperty("expiresAt")]
    public required long ExpiresAt { get; set; }
}
