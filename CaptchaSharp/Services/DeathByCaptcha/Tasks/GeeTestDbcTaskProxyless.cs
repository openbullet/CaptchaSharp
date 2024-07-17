using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks;

internal class GeeTestDbcTaskProxyless : DbcTaskProxyless
{
    [JsonProperty("gt")]
    public required string Gt { get; set; }
    
    [JsonProperty("challenge")]
    public required string Challenge { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
