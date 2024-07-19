using System;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.MetaBypassTech;

internal class MetaBypassTechAccessTokenResponse : MetaBypassTechResponse
{
    [JsonProperty("token_type")]
    public required string TokenType { get; set; }
    
    [JsonProperty("expires_in")]
    public required long ExpiresInSeconds { get; set; }
    
    [JsonProperty("access_token")]
    public required string AccessToken { get; set; }
    
    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; set; }
    
    public DateTime ExpirationDate { get; set; }

    public MetaBypassTechAccessTokenResponse()
    {
        // Subtract 1 minute to avoid expiration issues
        ExpirationDate = DateTime.Now.AddSeconds(ExpiresInSeconds) - TimeSpan.FromMinutes(1);
    }
}
