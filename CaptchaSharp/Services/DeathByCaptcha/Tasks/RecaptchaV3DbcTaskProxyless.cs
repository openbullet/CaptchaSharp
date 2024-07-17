using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks;

internal class RecaptchaV3DbcTaskProxyless : DbcTaskProxyless
{
    [JsonProperty("googlekey")]
    public required string GoogleKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("action")]
    public string? Action { get; set; }
    
    [JsonProperty("min_score")]
    public float MinScore { get; set; } = 0.3f;
}
