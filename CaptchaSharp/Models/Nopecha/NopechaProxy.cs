using Newtonsoft.Json;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaProxy
{
    [JsonProperty("scheme")]
    public required string Scheme { get; set; }
    
    [JsonProperty("host")]
    public required string Host { get; set; }
    
    [JsonProperty("port")]
    public int Port { get; set; }
    
    [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
    public string? Username { get; set; }
    
    [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
    public string? Password { get; set; }
}
