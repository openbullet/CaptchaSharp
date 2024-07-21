using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks.Proxied;

internal class GeeTestDbcTask : DbcTask
{
    [JsonProperty("gt")]
    public required string Gt { get; set; }
    
    [JsonProperty("challenge")]
    public required string Challenge { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
