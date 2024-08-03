using Newtonsoft.Json;

namespace CaptchaSharp.Models.MetaBypassTech;

internal class MbtSolveRecaptchaRequest
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("url")]
    public required string Url { get; set; }
    
    [JsonProperty("version")]
    public required string Version { get; set; }
}
