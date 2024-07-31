using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks.Proxied;

internal class CyberSiAraDbcTask : DbcTask
{
    [JsonProperty("slideurlid")]
    public required string SlideUrlId { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("useragent")]
    public required string UserAgent { get; set; }
}
