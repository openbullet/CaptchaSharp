using System;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.MetaBypassTech;

internal class MbtAccessTokenResponse : MbtResponse
{
    [JsonProperty("token_type")]
    public required string TokenType { get; set; }

    private long _expiresInSeconds;

    [JsonProperty("expires_in")]
    public required long ExpiresInSeconds
    {
        get => _expiresInSeconds;
        set
        {
            _expiresInSeconds = value;
            ExpirationDate = DateTime.UtcNow.AddSeconds(value) - TimeSpan.FromMinutes(1);
        }
    }

    [JsonProperty("access_token")]
    public required string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; set; }

    public DateTime ExpirationDate { get; set; }
}
