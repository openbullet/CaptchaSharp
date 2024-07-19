using Newtonsoft.Json;

namespace CaptchaSharp.Services.MetaBypassTech;

internal class MetaBypassTechRefreshAccessTokenRequest
{
    [JsonProperty("grant_type")]
    public required string GrantType { get; set; }
    
    [JsonProperty("client_id")]
    public required string ClientId { get; set; }
    
    [JsonProperty("client_secret")]
    public required string ClientSecret { get; set; }
    
    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; set; }
}
