using Newtonsoft.Json;

namespace CaptchaSharp.Models.MetaBypassTech;

internal class MbtAccessTokenRequest
{
    [JsonProperty("grant_type")]
    public required string GrantType { get; set; }
    
    [JsonProperty("client_id")]
    public required string ClientId { get; set; }
    
    [JsonProperty("client_secret")]
    public required string ClientSecret { get; set; }
    
    [JsonProperty("username")]
    public required string Username { get; set; }
    
    [JsonProperty("password")]
    public required string Password { get; set; }
}
