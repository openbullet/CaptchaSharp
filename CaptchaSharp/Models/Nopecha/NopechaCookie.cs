using Newtonsoft.Json;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaCookie
{
    [JsonProperty("name")]
    public required string Name { get; set; }
    
    [JsonProperty("value")]
    public required string Value { get; set; }
    
    [JsonProperty("domain")]
    public required string Domain { get; set; }
    
    [JsonProperty("path")]
    public required string Path { get; set; }
    
    [JsonProperty("hostOnly")]
    public required bool HostOnly { get; set; }
    
    [JsonProperty("httpOnly")]
    public required bool HttpOnly { get; set; }
    
    [JsonProperty("secure")]
    public required bool Secure { get; set; }
    
    [JsonProperty("session")]
    public required bool Session { get; set; }
    
    /// <summary>
    /// Seconds since UNIX epoch.
    /// </summary>
    [JsonProperty("expirationDate")]
    public required long ExpirationDate { get; set; }
}
